from openapi_server.models.base_model import Model

class UpdateIssueRequest:
    """
    @brief Ist eine Anfrage zur aktualisierung der Beschreibung eines Issues.

    @param description: Die neue Beschreibung des Issues.
    @type description: str
    """
    def __init__(self, description: str):
        self.description = description