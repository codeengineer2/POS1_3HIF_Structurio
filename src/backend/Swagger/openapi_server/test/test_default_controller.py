import unittest

from flask import json

from openapi_server.models.project import Project  # noqa: E501
from openapi_server.test import BaseTestCase


class TestDefaultController(BaseTestCase):
    """DefaultController integration test stubs"""

    def test_projects_get(self):
        """Test case for projects_get

        Liste aller Projekte
        """
        headers = { 
        }
        response = self.client.open(
            '/projects',
            method='GET',
            headers=headers)
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_projects_post(self):
        """Test case for projects_post

        Neues Projekt erstellen
        """
        project = {"createdAt":"2000-01-23T04:56:07.000+00:00","description":"description","id":"id","title":"title","status":"active","updatedAt":"2000-01-23T04:56:07.000+00:00"}
        headers = { 
            'Content-Type': 'application/json',
        }
        response = self.client.open(
            '/projects',
            method='POST',
            headers=headers,
            data=json.dumps(project),
            content_type='application/json')
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_projects_project_id_delete(self):
        """Test case for projects_project_id_delete

        Projekt löschen
        """
        headers = { 
        }
        response = self.client.open(
            '/projects/{project_id}'.format(project_id='project_id_example'),
            method='DELETE',
            headers=headers)
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_projects_project_id_get(self):
        """Test case for projects_project_id_get

        Einzelnes Projekt abrufen
        """
        headers = { 
        }
        response = self.client.open(
            '/projects/{project_id}'.format(project_id='project_id_example'),
            method='GET',
            headers=headers)
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_projects_project_id_put(self):
        """Test case for projects_project_id_put

        Projekt aktualisieren
        """
        project = {"createdAt":"2000-01-23T04:56:07.000+00:00","description":"description","id":"id","title":"title","status":"active","updatedAt":"2000-01-23T04:56:07.000+00:00"}
        headers = { 
            'Content-Type': 'application/json',
        }
        response = self.client.open(
            '/projects/{project_id}'.format(project_id='project_id_example'),
            method='PUT',
            headers=headers,
            data=json.dumps(project),
            content_type='application/json')
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))


if __name__ == '__main__':
    unittest.main()
