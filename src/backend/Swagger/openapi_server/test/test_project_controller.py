import unittest
from unittest.mock import patch, MagicMock
from openapi_server.controllers import project_controller

class TestProjectController(unittest.TestCase):

    @patch('openapi_server.controllers.project_controller.psycopg2.connect')
    @patch('openapi_server.controllers.project_controller.jsonify')
    def test_create_project_success(self, mock_jsonify, mock_connect):
        mock_conn = MagicMock()
        mock_cursor = MagicMock()
        mock_connect.return_value = mock_conn
        mock_conn.cursor.return_value = mock_cursor
        mock_cursor.fetchone.side_effect = [(1,), (10,)]
        mock_jsonify.return_value = "response"
        body = {
            "name": "Projekt",
            "description": "Beschreibung",
            "color": "#FFFFFF",
            "owner_uid": 123
        }
        result, code = project_controller.create_project(body)
        self.assertEqual(result, "response")
        self.assertEqual(code, 201)

    def test_create_project_missing_fields(self):
        body = {"name": "Projekt", "color": "#FFFFFF"}
        with self.assertRaises(Exception):
            project_controller.create_project(body)

    @patch('openapi_server.controllers.project_controller.psycopg2.connect')
    def test_create_project_db_exception(self, mock_connect):
        mock_conn = MagicMock()
        mock_cursor = MagicMock()
        mock_connect.return_value = mock_conn
        mock_conn.cursor.return_value = mock_cursor
        mock_cursor.execute.side_effect = Exception("Fehler")
        body = {
            "name": "Projekt",
            "description": "Beschreibung",
            "color": "#FFFFFF",
            "owner_uid": 123
        }
        with self.assertRaises(Exception):
            project_controller.create_project(body)

    @patch('openapi_server.controllers.project_controller.psycopg2.connect')
    @patch('openapi_server.controllers.project_controller.jsonify')
    def test_update_project_success(self, mock_jsonify, mock_connect):
        mock_conn = MagicMock()
        mock_cursor = MagicMock()
        mock_connect.return_value = mock_conn
        mock_conn.cursor.return_value = mock_cursor
        mock_cursor.fetchone.return_value = (1,)
        mock_jsonify.return_value = "ok"
        body = {
            "pid": 1,
            "name": "Neu",
            "description": "Neu",
            "color": "#000000"
        }
        result, code = project_controller.update_project(body)
        self.assertEqual(result, "ok")
        self.assertEqual(code, 200)

    def test_update_project_missing_fields(self):
        body = {"pid": 1, "name": "A"}
        with self.assertRaises(Exception):
            project_controller.update_project(body)

    @patch('openapi_server.controllers.project_controller.psycopg2.connect')
    def test_update_project_not_found(self, mock_connect):
        mock_conn = MagicMock()
        mock_cursor = MagicMock()
        mock_connect.return_value = mock_conn
        mock_conn.cursor.return_value = mock_cursor
        mock_cursor.fetchone.return_value = None
        body = {
            "pid": 1,
            "name": "Neu",
            "description": "Neu",
            "color": "#000000"
        }
        with self.assertRaises(Exception):
            project_controller.update_project(body)

    @patch('openapi_server.controllers.project_controller.psycopg2.connect')
    def test_update_project_exception(self, mock_connect):
        mock_conn = MagicMock()
        mock_cursor = MagicMock()
        mock_connect.return_value = mock_conn
        mock_conn.cursor.return_value = mock_cursor
        mock_cursor.execute.side_effect = Exception("Fehler")
        mock_cursor.fetchone.return_value = (1,)
        body = {
            "pid": 1,
            "name": "Neu",
            "description": "Neu",
            "color": "#000000"
        }
        with self.assertRaises(Exception):
            project_controller.update_project(body)

    @patch('openapi_server.controllers.project_controller.psycopg2.connect')
    @patch('openapi_server.controllers.project_controller.jsonify')
    def test_delete_project_success(self, mock_jsonify, mock_connect):
        mock_conn = MagicMock()
        mock_cursor = MagicMock()
        mock_connect.return_value = mock_conn
        mock_conn.cursor.return_value = mock_cursor
        mock_cursor.fetchone.return_value = (1,)
        mock_jsonify.return_value = "deleted"
        result, code = project_controller.delete_project(1)
        self.assertEqual(result, "deleted")
        self.assertEqual(code, 200)

    @patch('openapi_server.controllers.project_controller.psycopg2.connect')
    def test_delete_project_not_found(self, mock_connect):
        mock_conn = MagicMock()
        mock_cursor = MagicMock()
        mock_connect.return_value = mock_conn
        mock_conn.cursor.return_value = mock_cursor
        mock_cursor.fetchone.return_value = None
        with self.assertRaises(Exception):
            project_controller.delete_project(1)

    @patch('openapi_server.controllers.project_controller.psycopg2.connect')
    def test_delete_project_exception(self, mock_connect):
        mock_conn = MagicMock()
        mock_cursor = MagicMock()
        mock_connect.return_value = mock_conn
        mock_conn.cursor.return_value = mock_cursor
        mock_cursor.execute.side_effect = Exception("Fehler")
        mock_cursor.fetchone.return_value = (1,)
        with self.assertRaises(Exception):
            project_controller.delete_project(1)

if __name__ == '__main__':
    unittest.main()