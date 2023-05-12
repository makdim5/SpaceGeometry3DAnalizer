import json
import random

import pandas as pd
from matplotlib import pyplot as plt
from sklearn.cluster import AffinityPropagation, AgglomerativeClustering, Birch, KMeans, SpectralClustering, DBSCAN

generate_color = lambda: "#" + ''.join([random.choice('ABCDEF0123456789') for _ in range(6)])

nodes_df = pd.DataFrame(json.load(open("nodes.json", mode="r")))

fig = plt.figure()
ax = fig.add_subplot(111, projection='3d')
# ax.scatter(nodes_df["x"].values, nodes_df["y"].values, nodes_df["z"].values)
# plt.show()
# af = AffinityPropagation().fit(nodes_df)
# af = Birch().fit(nodes_df)
# af = KMeans(n_clusters=3, algorithm="elkan").fit(nodes_df)
af = DBSCAN(eps=2, min_samples=4).fit(nodes_df)
nodes_df["cluster"] = af.labels_

print(max(af.labels_) + 1)
# for i in range(max(af.labels_) + 1):
#     df = nodes_df[nodes_df["cluster"] == i]
#     ax.scatter(df["x"].values, df["y"].values, df["z"].values, c=generate_color())
#
# nodes_df.to_csv("nodes.csv")

# plt.show()

areas_to_send = [
    nodes_df[nodes_df["cluster"] == i].drop(["cluster"], axis=1).to_dict(orient='records')
    for i in range(max(af.labels_) + 1)
]

print(3)
