import pandas as pd
from sklearn.cluster import AgglomerativeClustering
import plotly.express as px

EXCEL_FILE_LOCATION = 'nodes.csv'
K_MAX = 15
K_MIN = 2
df = pd.read_csv(EXCEL_FILE_LOCATION)
clusterings = [AgglomerativeClustering(i).fit(df).labels_ for i in range(K_MIN, K_MAX + 1)]

for item in clusterings:
    df_cp = df.copy()
    df_cp["cluster"] = item
    fig = px.scatter_3d(df_cp, x='x', y='y', z='z',
                            color='cluster')
    fig.update_layout(
        title={
            'text': f"{max(item)+1}-clustering",
            'y': 0.9,
            'x': 0.5,
            'xanchor': 'center',
            'yanchor': 'top'})
    fig.show()

print(fig.to_html())