import pandas as pd
from flask import request, abort
from flask_restful import Resource

from sklearn.cluster import DBSCAN


class DensityBasedSpatialClustering(Resource):

    def post(self):
        try:
            print(f'{self.__class__.__name__} для кластеризации запущен...')
            nodes_df = pd.DataFrame(request.json)
            points_df = pd.DataFrame(nodes_df["point"].values.tolist())
            af = DBSCAN(eps=2, min_samples=4).fit(points_df)

            nodes_df["cluster"] = af.labels_

            areas_to_send = [
                nodes_df[nodes_df["cluster"] == i].drop(["cluster"], axis=1).to_dict(orient='records')
                for i in range(max(af.labels_) + 1)
            ]

            print("Кластеризация выполнена, области отправлены!")
            return areas_to_send
        except Exception:
            abort(400)
