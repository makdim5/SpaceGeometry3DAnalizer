import waitress
from flask import Flask
from flask_restful import Api

from resources.union_resource import Union
from resources.dbscan_resourse import DensityBasedSpatialClustering

flask_app = Flask(__name__)
api = Api(flask_app)

api.add_resource(Union, "/union", endpoint="union")
api.add_resource(DensityBasedSpatialClustering, "/dbscan", endpoint="dbscan")

if __name__ == "__main__":
    print("Сервер кластеризации запущен ...")
    waitress.serve(flask_app, port=5000, cleanup_interval=0)
