import unittest
from unittest.mock import patch, MagicMock
import psycopg2.errors
from openapi_server.controllers import user_controller

class TestUserController(unittest.TestCase):

    @patch('openapi_server.controllers.user_controller.get_connection')
    def test_auth_check_email_post_found(self, mock_conn):
        mock_cursor = MagicMock()
        mock_conn.return_value.cursor.return_value = mock_cursor
        mock_cursor.fetchone.return_value = (1,)
        response, status = user_controller.auth_check_email_post({"email": "test@example.com"})
        self.assertEqual(status, 200)
        self.assertEqual(response["exists"], True)

    @patch('openapi_server.controllers.user_controller.get_connection')
    def test_auth_check_email_post_not_found(self, mock_conn):
        mock_cursor = MagicMock()
        mock_conn.return_value.cursor.return_value = mock_cursor
        mock_cursor.fetchone.return_value = None
        response, status = user_controller.auth_check_email_post({"email": "notfound@example.com"})
        self.assertEqual(status, 404)
        self.assertEqual(response["error"], "E-Mail nicht gefunden")

    @patch('openapi_server.controllers.user_controller.get_connection')
    def test_auth_login_post_success(self, mock_conn):
        mock_cursor = MagicMock()
        mock_conn.return_value.cursor.return_value = mock_cursor

        mock_cursor.fetchone.side_effect = [
            (1, "Max", "Mustermann", "test@example.com", "2000-01-01"),
            (1,),
            (1, "Spalte"),
        ]
        mock_cursor.fetchall.side_effect = [
            [(1, "Projekt", "Beschreibung", "#000000")],
            [(1, "Spalte")],
            [(1, "Aufgabe")],
        ]

        response, status = user_controller.auth_login_post({
            "email": "test@example.com",
            "password": "pass"
        })

        self.assertEqual(status, 200)
        self.assertTrue(response["success"])
        self.assertEqual(response["user"]["firstname"], "Max")

    @patch('openapi_server.controllers.user_controller.get_connection')
    def test_auth_login_post_failed(self, mock_conn):
        mock_cursor = MagicMock()
        mock_conn.return_value.cursor.return_value = mock_cursor
        mock_cursor.fetchone.return_value = None
        response, status = user_controller.auth_login_post({
            "email": "fail@example.com",
            "password": "wrong"
        })
        self.assertEqual(status, 401)
        self.assertFalse(response["success"])

    @patch('openapi_server.controllers.user_controller.get_connection')
    def test_auth_register_post_success(self, mock_conn):
        mock_cursor = MagicMock()
        mock_conn.return_value.cursor.return_value = mock_cursor
        mock_cursor.fetchone.return_value = [1]

        response, status = user_controller.auth_register_post({
            "firstname": "Lisa",
            "lastname": "Muster",
            "email": "lisa@example.com",
            "password": "pw",
            "birthdate": "2000-01-01"
        })

        self.assertEqual(status, 201)
        self.assertTrue(response["success"])
        self.assertEqual(response["user"]["firstname"], "Lisa")

    @patch('openapi_server.controllers.user_controller.get_connection')
    def test_auth_register_post_duplicate(self, mock_conn):
        mock_cursor = MagicMock()
        mock_conn.return_value.cursor.return_value = mock_cursor
        mock_cursor.execute.side_effect = psycopg2.errors.UniqueViolation()

        response, status = user_controller.auth_register_post({
            "firstname": "Lisa",
            "lastname": "Muster",
            "email": "lisa@example.com",
            "password": "pw",
            "birthdate": "2000-01-01"
        })

        self.assertEqual(status, 409)
        self.assertFalse(response["success"])