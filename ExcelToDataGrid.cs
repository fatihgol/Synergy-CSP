using System;
using Bimser.CSP.FormControls.Common;
using Bimser.CSP.FormControls.Controls;
using Bimser.CSP.FormControls.Events;
using ExcelToDataGrid.Entities;
using Bimser.CSP.Workflow.EventArguments;
using Bimser.CSP.Workflow.Runtime.Models.Controller;
using Bimser.Synergy.ServiceAPI;
using Bimser.Synergy.Entities.DocumentManagement.Business.DTOs.Responses;
using Bimser.Synergy.ServiceAPI.Models.Authentication;
using Bimser.Synergy.ServiceAPI.Models.Workflow;
using Bimser.Synergy.ServiceAPI.Models.Form;
using System.Threading.Tasks;
using System.Net;
using Aspose.Cells;
using System.Data;
using System.IO;

namespace ExcelToDataGrid.Forms {

    public partial class Form1 {

		void RelatedDocuments1_OnAfterFileAdd(object sender, RelatedDocumentsAddEventArgs e)
		{
            if (e.Files != null)
            {
                foreach (var file in e.Files)
                {  
                    ServiceAPI service = new ServiceAPI(new Bimser.Synergy.ServiceAPI.Models.Authentication.LoginWithTokenAuthenticationParameters()
                    {
                        EncryptedData = Session.EncryptedData,
                        Language = Session.Language,
                        Token = Session.Token
                    }, "https://cloud333.bimser.net/api/web");

                    GetDownloadUrlResponse response = service.DocumentManagement.GetDownloadUrl(file.SecretKey, file.Name).Result;

                    string url = "https://cloud333.bimser.net/api/web/"+response.DownloadUrl;

                    byte[] excelBytes;

                    using (var webClient = new WebClient()) 
                    { 
                        excelBytes = webClient.DownloadData(url);  
                    }

                    Workbook wb = new Workbook(new MemoryStream(excelBytes));

                    //Get the first worksheet.
                    Worksheet worksheet = wb.Worksheets[0];

                    DataTable dt = worksheet.Cells.ExportDataTable(0, 0, worksheet.Cells.MaxDataRow, worksheet.Cells.MaxDataColumn + 1, new ExportTableOptions() { ExportColumnName = true });
                    
                    if(dt != null && dt.Rows != null && dt.Rows.Count > 0)
                    {
                        foreach(DataRow row in dt.Rows){
                            
                            DataGridRow rw = DataGrid1.NewRow();

                            rw["AdSoyad"].Text = row["FULLNAME"].ToString();
                            rw["Unvan"].Text = row["PROFESSION"].ToString();
                            rw["SurecSayisi"].Value = Convert.ToInt32(row["FLOWSTART"]);

                            DataGrid1.Rows.Add(rw);                            
                        }
                    }
                }
            }

		}


    }
}
