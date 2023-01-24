using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidServer.SolidWorksPackage.NodeWork;
using SolidServer.SolidWorksPackage.Simulation.FeatureFace;
using SolidServer.SolidWorksPackage.Simulation.MaterialWorker;
using SolidServer.SolidWorksPackage.Simulation.MeshWorker;
using SolidServer.SolidWorksPackage.Simulation.Study;
using SolidWorks.Interop.cosworks;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidServer.Simulation.Study
{
    public class StaticStudy
    {

        public CWStudy study;

        public ICWMesh mesh;

        private CWSolidManager solidManager;

        private ICWLoadsAndRestraintsManager restraintsManager;

        private CWSolidBody[] solidBodies;

        private List<ICWRestraint> fixedFaces;

        private List<CWForce> loadedFaces;

        private StaticStudyRecord record;

        private static swsLinearUnit_e LINEAR_UNIT = swsLinearUnit_e.swsLinearUnitMillimeters;

        public string MaterialName => solidManager.GetComponentAt(0, out int errorCode1).GetSolidBodyAt(0, out int errCode2).GetSolidBodyMaterial().MaterialName;

        public StaticStudy() { }

        

        public StaticStudy(CWStudy study, StaticStudyRecord record)
        {
            this.study = study;

            this.solidManager = study.SolidManager;

            this.restraintsManager = study.LoadsAndRestraintsManager;

            this.solidBodies = GetSolidBodies(this.solidManager);

            this.fixedFaces = new List<ICWRestraint>();

            this.loadedFaces = new List<CWForce>();

            this.record = record;

            LoadMaterial(record.material);

            int errorMesh = 0;
            int errorFix = 0;
            int errorLoad = 0;

            errorMesh = CreateMesh(record.mesh);
            errorFix = FixFaces(record.fixFaces.ToArray());
            errorLoad = LoadFaces(record.loadFaces.ToArray());

            StudyErrors(errorMesh, errorFix, errorLoad);
        }

        private static void StudyErrors(int errorMesh, int errorFix, int errorLoad)
        {


            bool error = false;
            string errorMsg = "";


            if (errorMesh != 0)
            {
                errorMsg += String.Format("Ошибка создания сетки : {0} !", errorMesh) + "\n";
                error = true;
            }


            if (errorFix != 0)
            {
                errorMsg += String.Format("Ошибка фикации сторон : {0} !", errorFix) + "\n";
                error = true;
            }

            if (errorLoad != 0)
            {
                errorMsg += String.Format("Ошибка нагрузки сторон : {0} !", errorLoad) + "\n";
                error = true;
            }

            if (error)
            {

                throw new Exception(errorMsg);

            }

        }

        public StaticStudy(CWStudy study)
        {
            this.study = study;
            this.solidManager = study.SolidManager;
            this.restraintsManager = study.LoadsAndRestraintsManager;
            this.solidBodies = GetSolidBodies(this.solidManager);
            mesh = study.Mesh;
        }

        public string GetMaterialName()
        {
            return solidManager.GetComponentAt(0, out int errorCode1).GetSolidBodyAt(0, out int errCode2).GetSolidBodyMaterial().MaterialName;
        }

        public int RunStudy()
        {

            int errorCode = 0;

            
            errorCode = study.RunAnalysis();

            return errorCode;

        }

        public StaticStudyResults GetResult()
        {
            return new StaticStudyResults(study);
        }

        public void LoadMaterial(Material material)
        {
            SetSolidBodyMaterial(this.solidBodies, material);
        }

        public int CreateDefaultMesh()
        {

            double averageGlobalElementSize = Mesh.DEFAULT_ELEMENT_SIZE;

            double tolerance = Mesh.DEFAULT_TOLERANCE;

            mesh = (ICWMesh)study.Mesh;

            mesh.Quality = 1;
            mesh.UseJacobianCheckForSolids = 2;
            mesh.MesherType = 1;
            mesh.MinElementsInCircle = 8;
            mesh.GrowthRatio = 1.4; 
            mesh.SaveSettingsWithoutMeshing = 0;
            mesh.Unit = 0;

            mesh.GetDefaultElementSizeAndTolerance(
                (int)LINEAR_UNIT,
                out averageGlobalElementSize,
                out tolerance);

            int errorCode = study.CreateMesh(
                (int)LINEAR_UNIT,
                averageGlobalElementSize,
                tolerance);

            return errorCode;
        }

        public int CreateMesh(double averageGlobalElementSize, double tolerance)
        {

            mesh = (ICWMesh)study.Mesh;

            mesh.Quality = (int)swsMeshQuality_e.swsMeshQualityHigh;


            int errorCode = study.CreateMesh(
                (int)LINEAR_UNIT,
                averageGlobalElementSize,
                tolerance);

            return errorCode;
        }

        public int CreateMesh(Mesh stdMesh)
        {
            return CreateMesh(stdMesh.averageGlobalElementSize, stdMesh.tolerance);
        }

        public int FixFaces(FeatureFace[] faces)
        {
            object[] objectFaces = ConvertFacesToObjects(faces);

            return FixFaces(fixedFaces, restraintsManager, objectFaces);

        }

        public int LoadFaces(FeatureFace[] faces)
        {

            object[] objectFaces = ConvertFacesToObjects(faces);

            int errorCode = 0;

            foreach (var face in faces)
            {

                object[] objFace = new object[] { face.face as object };

                errorCode = LoadFaces(loadedFaces, restraintsManager, objFace, face.force);

            }

            return errorCode;

        }

        private static int FixFaces(List<ICWRestraint> fixedFaces, ICWLoadsAndRestraintsManager restraintsManager, object[] faces)
        {

            int errorCode = 0;

            ICWRestraint restraint = restraintsManager.AddRestraint(
                (int)swsRestraintType_e.swsRestraintTypeFixed,
                faces,
                null,
                out errorCode);

            if (errorCode == 0)
            {
                fixedFaces.Add(restraint);
            }

            return errorCode;
        }

        private static int LoadFaces(List<CWForce> loadedFaces, ICWLoadsAndRestraintsManager restraintsManager, object[] faces, double value)
        {

            int errorCode = 0;

            double[] COMPS = { 1, 1, 1, 1, 1, 1 };
            double[] DIST_VALUE = new double[0];
            double[] FORCE_VALUE = new double[0];
            int UCODE = 0;
            int refForceDirection = -1;
            int notUsed = 0;

            CWForce force = restraintsManager.AddForce3(
                (int)swsForceType_e.swsForceTypeNormal,
                (int)swsSelectionType_e.swsSelectionFaceEdgeVertexPoint,
                refForceDirection,
                notUsed,
                notUsed,
                notUsed,
                DIST_VALUE,
                FORCE_VALUE,
                true,
                false,
                notUsed,
                notUsed,
                UCODE,
                value,
                COMPS,
                false,
                false,
                faces,
                null,
                false,
                out errorCode);

            if (errorCode == 0)
            {
                loadedFaces.Add(force);
            }

            return errorCode;

        }

        private static void SetSolidBodyMaterial(CWSolidBody[] solidBodies, Material material)
        {

            foreach (CWSolidBody solidBody in solidBodies)
            {

                CWMaterial cwmaterial = solidBody.GetDefaultMaterial();

                cwmaterial.MaterialUnits = 0;

                cwmaterial.MaterialName = material.name;

                foreach (string physicalPropertieName in material.physicalProperties.Keys)
                {

                    cwmaterial.SetPropertyByName(physicalPropertieName, material.physicalProperties[physicalPropertieName], 0);

                }

                solidBody.SetSolidBodyMaterial(cwmaterial);

            }

        }

        private static CWSolidBody[] GetSolidBodies(CWSolidManager solidManager)
        {

            List<CWSolidBody> result = new List<CWSolidBody>();

            int errorCode = 0;

            for (int i = 0; i < solidManager.ComponentCount; i++)
            {

                CWSolidComponent solidComponent = solidManager.GetComponentAt(i, out errorCode);

                CWSolidBody solidBody = solidComponent.GetSolidBodyAt(0, out errorCode);

                result.Add(solidBody);

            }



            //errCode = SolidBody.SetSolidBodyMaterial(material.GetCWMaterial());


            return result.ToArray();
        }

        private object[] ConvertFacesToObjects(FeatureFace[] faces)
        {

            List<object> result = new List<object>();

            foreach (var face in faces)
            {

                result.Add(face.face as object);

            }

            return result.ToArray();
        }
    }
}
