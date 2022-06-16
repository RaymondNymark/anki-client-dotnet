using AnkiWeb.Client.Common.Models;
using AnkiWeb.Client.Helpers;

namespace AnkiWeb.Client;
public interface IAnkiClient
{
    Task<AnkiClientStatus> AddNewCardToDeckAsync(Card card, string deckId);
    Task<(CollectionInfo collectionInfo, AnkiClientStatus status)> GetCollectionInfoAsync();
}
