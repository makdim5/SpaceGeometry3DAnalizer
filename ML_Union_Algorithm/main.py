from flask import Flask
from flask_restful import Api

from resources.union_resource import Union

app = Flask(__name__)
api = Api(app)

api.add_resource(Union, "/union", endpoint="union")

if __name__ == "__main__":

    app.run()
