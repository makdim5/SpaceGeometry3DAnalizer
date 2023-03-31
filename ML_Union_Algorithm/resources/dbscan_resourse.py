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
            min_border_squeeze_coefficient = 0.01
            max_border_squeeze_coefficient = 0.01
            for i in range(max(af.labels_) + 1):
                area = {}
                df = nodes_df[nodes_df["cluster"] == i]
                area["minX"] = min(df["x"].values) + min_border_squeeze_coefficient*abs(min(df["x"].values))
                area["maxX"] = max(df["x"].values) - max_border_squeeze_coefficient*abs(max(df["x"].values))
                area["minY"] = min(df["y"].values) + min_border_squeeze_coefficient*abs(min(df["y"].values))
                area["maxY"] = max(df["y"].values) - max_border_squeeze_coefficient*abs(max(df["y"].values))
                area["minZ"] = min(df["z"].values) + min_border_squeeze_coefficient*abs(min(df["z"].values))
                area["maxZ"] = max(df["z"].values) - max_border_squeeze_coefficient*abs(max(df["z"].values))

                x_delta = abs(area["maxX"] - area["minX"])
                y_delta = abs(area["maxY"] - area["minY"])
                z_delta = abs(area["maxY"] - area["minY"])

                threshold = 0.5
                if x_delta > threshold and y_delta > threshold and z_delta > threshold:
                    areas_to_send.append(area)

            print("Кластеризация выполнена, области отправлены!")
            return areas_to_send
        except Exception:
            abort(400)
