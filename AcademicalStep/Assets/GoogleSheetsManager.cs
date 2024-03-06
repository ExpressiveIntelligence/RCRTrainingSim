using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;


public class GoogleSheetsManager : MonoBehaviour
{
    
    static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    static readonly string ApplicationName = "academical";
    static readonly string SpreadsheetId = "1rgajs7U8nwZRaVcEmtKjKtO6DRxI484UCz8f4shHRMk";
    static readonly string sheet = "academical";


    private SheetsService service;
    public TextAsset credentials;

    private void Awake()
    {
        GoogleCredential credential;
        credential = GoogleCredential.FromJson(credentials.text).CreateScoped(Scopes);


        // Create the Sheets service.
        this.service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

    void Start()
    {
    }

    public void SavePlaythroughData(string sheetName, List<SerializedFragment> fragments)
    {
        var values = new List<IList<object>>();
        foreach(var fragment in fragments) 
        {
            values.Add(new List<object> {
                fragment.fragmentID,
                fragment.content
            });
        }
        StartCoroutine(CreateSheetAndSavePlaythroughData(sheetName, values));
    }

    IEnumerator CreateSheetAndSavePlaythroughData(string sheetName, List<IList<object>> values) 
    { 
        yield return CreateNewSheet(sheetName);
        yield return SaveFragmentValues(sheetName, values);
    }

    IEnumerator SaveFragmentValues(string sheetName, List<IList<object>> values) 
    {
        var range = sheetName + "!A1";
        ValueRange body = new ValueRange
        {
            Values = values
        };

        var update = service.Spreadsheets.Values.Update(body, SpreadsheetId, range);
        update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

        yield return update.Execute();
    }

    IEnumerator CreateNewSheet(string sheetName) 
    {
        // Add new Sheet
        var addSheetRequest = new AddSheetRequest();
        addSheetRequest.Properties = new SheetProperties();
        addSheetRequest.Properties.Title = sheetName;
        BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
        batchUpdateSpreadsheetRequest.Requests = new List<Request>();
        batchUpdateSpreadsheetRequest.Requests.Add(new Request
        {
            AddSheet = addSheetRequest
        });

        var batchUpdateRequest =
            this.service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, SpreadsheetId);

        yield return batchUpdateRequest.Execute();
    }



}
