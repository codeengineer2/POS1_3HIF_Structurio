import connexion
from typing import Dict
from typing import Tuple
from typing import Union

from openapi_server.models.project import Project  # noqa: E501
from openapi_server import util


def projects_get():  # noqa: E501
    """Liste aller Projekte

     # noqa: E501


    :rtype: Union[None, Tuple[None, int], Tuple[None, int, Dict[str, str]]
    """
    return 'do some magic!'


def projects_post(body):  # noqa: E501
    """Neues Projekt erstellen

     # noqa: E501

    :param project: 
    :type project: dict | bytes

    :rtype: Union[None, Tuple[None, int], Tuple[None, int, Dict[str, str]]
    """
    project = body
    if connexion.request.is_json:
        project = Project.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'


def projects_project_id_delete(project_id):  # noqa: E501
    """Projekt löschen

     # noqa: E501

    :param project_id: 
    :type project_id: str

    :rtype: Union[None, Tuple[None, int], Tuple[None, int, Dict[str, str]]
    """
    return 'do some magic!'


def projects_project_id_get(project_id):  # noqa: E501
    """Einzelnes Projekt abrufen

     # noqa: E501

    :param project_id: 
    :type project_id: str

    :rtype: Union[None, Tuple[None, int], Tuple[None, int, Dict[str, str]]
    """
    return 'do some magic!'


def projects_project_id_put(project_id, body):  # noqa: E501
    """Projekt aktualisieren

     # noqa: E501

    :param project_id: 
    :type project_id: str
    :param project: 
    :type project: dict | bytes

    :rtype: Union[None, Tuple[None, int], Tuple[None, int, Dict[str, str]]
    """
    project = body
    if connexion.request.is_json:
        project = Project.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'
