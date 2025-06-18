import unittest
from unittest.mock import patch, MagicMock
from openapi_server.controllers import column_controller
import psycopg2
from flask import abort
from openapi_server import app

class TestColumnController(unittest.TestCase):

    @patch('openapi_server.controllers.column_controller.get_connection')
    def test_add_column_success(self, mock_conn):
        mock_cursor = MagicMock()
        mock_conn.return_value.cursor.return_value = mock_cursor
        mock_cursor.fetchone.return_value = [123]

        response, status = column_controller.add_column({"board_id": 1, "name": "ToDo"})
        self.assertEqual(status, 201)
        self.assertEqual(response.json["cid"], 123)
        self.assertEqual(response.json["name"], "ToDo")

    @patch('openapi_server.controllers.column_controller.abort')
    def test_add_column_missing_fields(self, mock_abort):
        column_controller.add_column({"board_id": None, "name": None})
        mock_abort.assert_called_with(400, "board_id und name sind erforderlich.")

    @patch('openapi_server.controllers.column_controller.get_connection')
    def test_add_column_db_error(self, mock_conn):
        mock_cursor = MagicMock()
        mock_cursor.execute.side_effect = Exception("Fehler")
        mock_conn.return_value.cursor.return_value = mock_cursor

        with self.assertRaises(Exception):
            column_controller.add_column({"board_id": 1, "name": "Fehler"})

    @patch('openapi_server.controllers.column_controller.get_connection')
    def test_update_column_success(self, mock_conn):
        mock_cursor = MagicMock()
        mock_conn.return_value.cursor.return_value = mock_cursor
        mock_cursor.rowcount = 1

        response, status = column_controller.update_column({"id": 5, "name": "Done"})
        self.assertEqual(status, 200)
        self.assertTrue(response.json["success"])

    @patch('openapi_server.controllers.column_controller.abort')
    def test_update_column_missing_fields(self, mock_abort):
        column_controller.update_column({"id": None, "name": None})
        mock_abort.assert_called_with(400, "id und name sind erforderlich.")

    @patch('openapi_server.controllers.column_controller.get_connection')
    def test_update_column_not_found(self, mock_conn):
        mock_cursor = MagicMock()
        mock_cursor.rowcount = 0
        mock_conn.return_value.cursor.return_value = mock_cursor

        with self.assertRaises(Exception):
            column_controller.update_column({"id": 999, "name": "Unbekannt"})

    @patch('openapi_server.controllers.column_controller.get_connection')
    def test_update_column_error(self, mock_conn):
        mock_cursor = MagicMock()
        mock_cursor.execute.side_effect = Exception("Datenbankfehler")
        mock_conn.return_value.cursor.return_value = mock_cursor

        with self.assertRaises(Exception):
            column_controller.update_column({"id": 1, "name": "Fehler"})