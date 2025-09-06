using DocuWare.Platform.ServerClient;
using EasierDocuware.Models;
using EasierDocuware.Utils;
using Microsoft.IdentityModel.Tokens;
using Document = DocuWare.Platform.ServerClient.Document;


namespace EasierDocuware.Services
{
  public class DocuwareService
  {
    private ServiceConnection _serviceConnection;
    private readonly DocuwareUtils _utils;

    public DocuwareService(ServiceConnection serviceConnection, DocuwareUtils utils)
    {
      _serviceConnection = serviceConnection;
      _utils = utils;
    }

    public void Initialize()
    {
    }


    // falta testejar tot un cop hagi un dw disponible !!!



    public async Task<ServiceResult<bool>> ConnectAsync(string url, string username, string password)
    {
      try
      {
        var connection = await ServiceConnection.CreateAsync(new Uri(url), username, password).ConfigureAwait(false);
        if (connection == null) return ServiceResult<bool>.Fail("Connection failed!");

        return ServiceResult<bool>.Ok(true);
      }
      catch (Exception ex)
      {
        return ServiceResult<bool>.Fail(ex.Message);
      }
    }


    public ServiceResult<FileCabinet> GetFileCabinetById(string fileCabinetId)
    {
      try
      {
        if (string.IsNullOrEmpty(fileCabinetId)) return ServiceResult<FileCabinet>.Fail("File cabinet id cannot be null!");

        var fileCabinet = _serviceConnection.GetFileCabinet(fileCabinetId);
        if (fileCabinet == null) return ServiceResult<FileCabinet>.Fail("File cabinet not found!");

        return ServiceResult<FileCabinet>.Ok(fileCabinet);
      }
      catch (Exception ex)
      {
        return ServiceResult<FileCabinet>.Fail(ex.Message);
      }
    }

    public async Task<ServiceResult<List<FileCabinet>>> GetFileCabinetsAsync()
    {
      try
      {
        var org = _serviceConnection.Organizations[0];
        if (org == null) return ServiceResult<List<FileCabinet>>.Fail("No organization found!");

        var response = await org.GetFileCabinetsFromFilecabinetsRelationAsync();
        if (!response.IsSuccessStatusCode) return ServiceResult<List<FileCabinet>>.Fail($"Get file cabinets failed with status code {response.StatusCode}: {response.Exception.Message}");

        return ServiceResult<List<FileCabinet>>.Ok(response.Content.FileCabinet);
      }
      catch (Exception ex)
      {
        return ServiceResult<List<FileCabinet>>.Fail(ex.Message);
      }
    }



    // FALTA IMPLEMENTAR GESTIÓ D ERRORS AMB LA REPOSTA HTTP DE DW
    public async Task<ServiceResult<List<Document>>> GetDocumentsByFileCabinetIdAsync(string fileCabinetId, int? count = 10000)
    {
      try
      {
        DocumentsQueryResult queryResult = await _serviceConnection.GetFromDocumentsForDocumentsQueryResultAsync(fileCabinetId, count: count).ConfigureAwait(false);

        List<Document> result = new List<Document>();
        await _utils.GetAllDocumentsAsync(queryResult, result);

        return ServiceResult<List<Document>>.Ok(result);
      }
      catch (Exception ex)
      {
        return ServiceResult<List<Document>>.Fail(ex.Message);
      }
    }

    public async Task<ServiceResult<bool>> UpdateDocFieldsAsync(Document doc, Dictionary<string, string> fields, bool forceUpdate)
    {
      try
      {
        if (doc == null || fields.IsNullOrEmpty()) return ServiceResult<bool>.Fail("Parameteres cannot be null!");

        var docFields = new List<DocumentIndexField>();
        foreach (var field in fields) docFields.Add(DocumentIndexField.Create(field.Key, field.Value));

        var updateIndexFieldsInfo = new UpdateIndexFieldsInfo()
        {
          Field = docFields,
          ForceUpdate = forceUpdate
        };

        var response = await doc.PutToFieldsRelationForDocumentIndexFieldsAsync(updateIndexFieldsInfo);
        if (!response.IsSuccessStatusCode) return ServiceResult<bool>.Fail($"Update failed with status code {response.StatusCode}: {response.Exception.Message}");

        return ServiceResult<bool>.Ok(true);
      }
      catch (Exception ex)
      {
        return ServiceResult<bool>.Fail(ex.Message);
      }
    }

    public async Task<ServiceResult<bool>> BatchUpdateDocFieldsAsync(string fileCabinetId, List<int> documentIds, Dictionary<string, string> fields, bool forceUpdate)
    {
      try
      {
        if (documentIds.IsNullOrEmpty() || fields.IsNullOrEmpty() || fileCabinetId.IsNullOrEmpty()) return ServiceResult<bool>.Fail("Parameteres cannot be null!");

        var docFields = new List<DocumentIndexField>();
        foreach (var field in fields) docFields.Add(DocumentIndexField.Create(field.Key, field.Value));

        var batchUpdateProcess = new BatchUpdateProcess
        {
          Data = new BatchUpdateProcessData
          {
            Field = docFields,
            ForceUpdate = forceUpdate
          },
          Source = new BatchUpdateDocumentsSource
          {
            Id = documentIds
          }
        };

        var fileCabinet = GetFileCabinetById(fileCabinetId);

        var response = await fileCabinet.Data!.PostToBatchUpdateRelationForBatchUpdateIndexFieldsResultAsync(batchUpdateProcess);
        if (!response.IsSuccessStatusCode) return ServiceResult<bool>.Fail($"Batch update failed with status code {response.StatusCode}: {response.Exception.Message}");

        return ServiceResult<bool>.Ok(true);
      }
      catch (Exception ex)
      {
        return ServiceResult<bool>.Fail(ex.Message);
      }
    }



    // FALTA IMPLEMENTAR AQUESTA FUNCIÓ 
    public static void ForceUpdateBatchAppendKeywordIndexFields(FileCabinet fileCabinet)
    {
      List<int> documentIds = new List<int>()
            {
                1, 2, 3, 4
            };

      List<string> keywordValues = new List<string>()
            {
                "Café", "Expensive", "Tasty"
            };

      var batchAppendKeywordValues = new BatchAppendKeywordValues
      {
        DocId = documentIds,
        FieldName = "Keyword", //the name of the keyword fields
        Keyword = keywordValues,
        StoreDialogId = "storeDialogId",
        ForceUpdate = false
      };

      //In this case if values of the fields match the existing ones, the batch append keyword and audit for it will be skipped
      fileCabinet.PostToBatchUpdateRelationForBatchUpdateIndexFieldsResult(batchAppendKeywordValues);

      batchAppendKeywordValues = new BatchAppendKeywordValues
      {
        BreakOnError = false,
        DocId = documentIds,
        FieldName = "Keyword", //the name of the keyword fields
        Keyword = keywordValues,
        StoreDialogId = "storeDialogID",
        ForceUpdate = true
      };

      //In this case if values of the fields match the existing ones, the batch append keyword and audit for it will happen
      fileCabinet.PostToBatchUpdateRelationForBatchUpdateIndexFieldsResult(batchAppendKeywordValues);
    }
  }
}
