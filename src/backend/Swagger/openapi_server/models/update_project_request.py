from openapi_server.models.base_model import Model

class UpdateProjectRequest(Model):
    """
    @brief Ist eine Anfrage zur aktualisierung eines Projekts.

    @param pid: Die Project-ID die aktualisiert werden soll.
    @type pid: int
    @param name: Der neue Name des Projekts.
    @type name: str
    @param description: Die neue Beschreibung des Projekts.
    @type description: str
    @param color: Die neue Farbe für das Projekt.
    @type color: str
    """
    def __init__(self, pid=None, name=None, description=None, color=None):
        self.openapi_types = {
            'pid': int,
            'name': str,
            'description': str,
            'color': str
        }

        self.attribute_map = {
            'pid': 'pid',
            'name': 'name',
            'description': 'description',
            'color': 'color'
        }

        self.pid = pid
        self.name = name
        self.description = description
        self.color = color