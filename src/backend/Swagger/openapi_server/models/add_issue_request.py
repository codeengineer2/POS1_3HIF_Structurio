from openapi_server.models.base_model import Model

class AddIssueRequest:
    """
    @brief Ist eine Anfrage zum erstellen eines neuen Issues.

    @param column_id: Die ID der Spalte dem das Issue hinzugefügt wird.
    @type column_id: int
    @param description: Die Beschreibung des Issues.
    @type description: str
    """
    def __init__(self, column_id: int, description: str):
        self.column_id = column_id
        self.description = description