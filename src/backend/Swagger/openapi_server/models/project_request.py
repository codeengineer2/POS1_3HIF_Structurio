from openapi_server.models.base_model import Model

class ProjectRequest(Model):
    """
    @brief Ist eine Anfrage zum Erstellen eines neuen Projekts.

    @param name: Der Name des Projekts.
    @type name: str

    @param description: Eine Beschreibung des Projekts.
    @type description: str

    @param color: Die Farbe des Projekts.
    @type color: str

    @param owner_uid: Die User ID des Projektinhabers.
    @type owner_uid: int
    """
    def __init__(self, name=None, description=None, color=None, owner_uid=None):
        self.openapi_types = {
            'name': str,
            'description': str,
            'color': str,
            'owner_uid': int
        }

        self.attribute_map = {
            'name': 'name',
            'description': 'description',
            'color': 'color',
            'owner_uid': 'owner_uid'
        }

        self.name = name
        self.description = description
        self.color = color
        self.owner_uid = owner_uid