from openapi_server.models.base_model import Model

class UpdateColumnRequest(Model):
    """
    @brief Ist eine Anfrage zur aktualisierung des Namens einer Spalte.

    @param id: Die ID der Spalte.
    @type id: int
    @param name: Der neue Name der Spalte.
    @type name: str
    """
    def __init__(self, id=None, name=None):
        self.openapi_types = {'id': int, 'name': str}
        self.attribute_map = {'id': 'id', 'name': 'name'}

        self.id = id
        self.name = name