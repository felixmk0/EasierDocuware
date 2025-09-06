using DocuWare.Platform.ServerClient;


namespace EasierDocuware.Utils
{
  public class DocuwareUtils
  {

    public DocuwareUtils()
    {
        
    }


    public async Task GetAllDocumentsAsync(DocumentsQueryResult queryResult, List<Document> documents)
    {
      documents.AddRange(queryResult.Items);

      if (queryResult.NextRelationLink != null)
      {
        await GetAllDocumentsAsync(await queryResult.GetDocumentsQueryResultFromNextRelationAsync(), documents);
      }
    }
  }
}
