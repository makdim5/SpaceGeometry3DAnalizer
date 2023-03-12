from typing import Callable, List

from model.Cluster import Cluster


class MatrixHelper:
    @staticmethod
    def create_square_matrix_per_rule(function_rule: Callable, objects: List[Cluster]) \
            -> List[List[float | int]]:
        return [[function_rule(objects[i].get_center(), objects[j].get_center())
                 for j in range(len(objects))]
                for i in range(len(objects))]

    @staticmethod
    def remove_matrix_row(matrix: List[List[float | int]], row_number: int) \
            -> List[List[float | int]]:
        if row_number not in range(len(matrix)):
            raise Exception("row number not in matrix")

        matrix.remove(matrix[row_number])
        return matrix

    @staticmethod
    def remove_matrix_column(matrix: List[List[float | int]], column_number: int) \
            -> List[List[float | int]]:
        if column_number not in range(len(matrix)):
            raise Exception("column number not in matrix")

        return [[matrix[i][j] for j in range(len(matrix)) if j != column_number]
                for i in range(len(matrix))]

    @staticmethod
    def find_min_element_coords_in_matrix(matrix):
        min_i, min_j = 0, 1

        for i in range(len(matrix)):
            for j in range(len(matrix[0])):
                if matrix[i][j] < matrix[min_i][min_j] and i != j:
                    min_i, min_j = i, j

        return min_i, min_j
