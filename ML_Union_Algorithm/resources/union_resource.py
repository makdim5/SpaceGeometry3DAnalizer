from flask import request, abort
from flask_restful import Resource

from model.clusterization import *
from model.Point import Point


class Union(Resource):

    def post(self):
        try:
            print('UnionService для кластеризации запущен...')
            data = request.json
            points = [Point(tuple([value["x"], value["y"], value["z"]]), number) for number, value in enumerate(data)]
            union_claster_worker = UnionClusterizationWorker(points)
            global_clusterization = union_claster_worker.make_clusterization(EXTERNAL_K_MAX, EXTERNAL_K_MIN)
            print("Этап глобальной кластеризации выполнен")
            ready_clusters = []
            for item in global_clusterization:
                union_claster_worker.worker.points = item.points
                ready_clusters.extend(union_claster_worker.make_clusterization(INTERNAL_K_MAX, INTERNAL_K_MIN))
            print("Этап кластеризации каждого кластера выполнен")
            spheres_list = []

            for cluster in ready_clusters:
                center = cluster.get_center()
                rad = cluster.get_internal_cluster_distance()
                if rad:
                    spheres_list.append(
                        {"center": {
                            "x": center.attributes[0],
                            "y": center.attributes[1],
                            "z": center.attributes[2]
                        }, "radious": rad/1000}
                    )

            print("Кластеризация выполнена, области отправлены!")
            return {"spheres": spheres_list}
        except Exception:
            abort(400)
