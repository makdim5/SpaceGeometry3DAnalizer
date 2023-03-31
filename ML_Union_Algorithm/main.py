from flask import Flask
from flask_restful import Api

from resources.union_resource import Union
from resources.dbscan_resourse import DensityBasedSpatialClustering

app = Flask(__name__)
api = Api(app)

api.add_resource(Union, "/union", endpoint="union")
api.add_resource(DensityBasedSpatialClustering, "/dbscan", endpoint="dbscan")

if __name__ == "__main__":

    app.run()
