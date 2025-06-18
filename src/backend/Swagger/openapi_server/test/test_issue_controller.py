import unittest
from unittest.mock import patch, MagicMock
from openapi_server.controllers import issue_controller
import psycopg2
from openapi_server import app

class TestIssueController(unittest.TestCase):

    @patch('openapi_server.controllers.issue_controller.get_connection')
    def test_add_issue_success(self, mock_conn):
        mock_cursor = MagicMock()
        mock_conn.return_value.cursor.return_value = mock_cursor
        mock_cursor.fetchone.return_value = [100]

        response, status = issue_controller.add_issue({
            "description": "Test-Issue",
            "column_id": 1
        })

        self.assertEqual(status, 201)
        self.assertTrue(response.json["success"])
        self.assertEqual(response.json["issue_id"], 100)

    @patch('openapi_server.controllers.issue_controller.abort')
    def test_add_issue_missing_fields(self, mock_abort):
        issue_controller.add_issue({"description": "", "column_id": None})
        mock_abort.assert_called_with(400, "description und column_id sind erforderlich.")

    @patch('openapi_server.controllers.issue_controller.get_connection')
    def test_add_issue_sql_error(self, mock_conn):
        mock_cursor = MagicMock()
        mock_cursor.execute.side_effect = Exception("Fehler")
        mock_conn.return_value.cursor.return_value = mock_cursor

        with self.assertRaises(Exception):
            issue_controller.add_issue({"description": "Fehler", "column_id": 1})

    @patch('openapi_server.controllers.issue_controller.get_connection')
    def test_update_issue_success(self, mock_conn):
        mock_cursor = MagicMock()
        mock_conn.return_value.cursor.return_value = mock_cursor

        response, status = issue_controller.update_issue({
            "id": 1,
            "description": "Aktualisiert"
        })

        self.assertEqual(status, 200)
        self.assertTrue(response.json["success"])

    @patch('openapi_server.controllers.issue_controller.abort')
    def test_update_issue_missing_fields(self, mock_abort):
        issue_controller.update_issue({"id": None, "description": None})
        mock_abort.assert_called_with(400, "issue_id und description sind erforderlich.")

    @patch('openapi_server.controllers.issue_controller.get_connection')
    def test_update_issue_sql_error(self, mock_conn):
        mock_cursor = MagicMock()
        mock_cursor.execute.side_effect = Exception("Fehler")
        mock_conn.return_value.cursor.return_value = mock_cursor

        with self.assertRaises(Exception):
            issue_controller.update_issue({"id": 1, "description": "Fehler"})

    @patch('openapi_server.controllers.issue_controller.get_connection')
    def test_delete_issue_success(self, mock_conn):
        mock_cursor = MagicMock()
        mock_cursor.rowcount = 1
        mock_conn.return_value.cursor.return_value = mock_cursor

        response, status = issue_controller.delete_issue(1)
        self.assertEqual(status, 200)
        self.assertTrue(response.json["success"])

    @patch('openapi_server.controllers.issue_controller.get_connection')
    def test_delete_issue_not_found(self, mock_conn):
        mock_cursor = MagicMock()
        mock_cursor.rowcount = 0
        mock_conn.return_value.cursor.return_value = mock_cursor

        with self.assertRaises(Exception):
            issue_controller.delete_issue(999)

    @patch('openapi_server.controllers.issue_controller.get_connection')
    def test_delete_issue_sql_error(self, mock_conn):
        mock_cursor = MagicMock()
        mock_cursor.execute.side_effect = Exception("Fehler")
        mock_conn.return_value.cursor.return_value = mock_cursor

        with self.assertRaises(Exception):
            issue_controller.delete_issue(1)