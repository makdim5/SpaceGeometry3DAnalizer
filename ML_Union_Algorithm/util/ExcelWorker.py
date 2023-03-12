from typing import List, Tuple
from pandas import DataFrame, read_excel, ExcelWriter

from model.Point import Point


class ExcelWorker:
    @staticmethod
    def read(filepath: str, sheet_name: str = 'Sheet') -> DataFrame:
        return read_excel(filepath, sheet_name=sheet_name)

    @staticmethod
    def rewrite(filepath: str, dataframe: DataFrame, sheet_name: str = "Sheet") -> None:
        dataframe.to_excel(filepath, sheet_name=sheet_name, index=False)

    @staticmethod
    def append_df_to_existing_file(filepath: str, df: DataFrame) -> None:
        with ExcelWriter(filepath, mode='a') as writer:
            df.to_excel(writer)

    @staticmethod
    def define_points_in_df(df: DataFrame, attributes: Tuple[str]) -> List[Point]:
        return [Point(tuple(map(lambda attr: float(df[attr][i]), attributes)),
                      i) for i in range(len(df[attributes[0]]))]

    @staticmethod
    def define_clustered_points_in_df(df: DataFrame, attributes: Tuple[str]) -> List[Point]:
        if "class" not in list(df):
            raise Exception("\'class\' field not found!")
        return [Point(tuple(map(lambda attr: float(df[attr][i]), attributes)),
                      i) for i in range(len(df[attributes[0]]))]
