import pandas as pd
from flask import request, abort
from flask_restful import Resource

from sklearn.cluster import DBSCAN


class DensityBasedSpatialClustering(Resource):

    def post(self):
        try:
            print('DensityBasedSpatialClustering для кластеризации запущен...')

            nodes_df = pd.DataFrame(request.json)

            af = DBSCAN(eps=2, min_samples=4).fit(nodes_df)
            nodes_df["cluster"] = af.labels_

            areas_to_send = []
            squeeze_coefficient = 0.8
            for i in range(max(af.labels_) + 1):
                area = {}
                df = nodes_df[nodes_df["cluster"] == i]
                area["minX"] = min(df["x"].values)
                area["minY"] = min(df["y"].values)
                area["minZ"] = min(df["z"].values)
                area["maxX"] = max(df["x"].values) * squeeze_coefficient
                area["maxY"] = max(df["y"].values) * squeeze_coefficient
                area["maxZ"] = max(df["z"].values) * squeeze_coefficient
                areas_to_send.append(area)

            print("Кластеризация выполнена, области отправлены!")
            return areas_to_send
        except Exception:
            abort(400)
