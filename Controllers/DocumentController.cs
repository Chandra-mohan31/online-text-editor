using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Online_Text_editor.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Text;

namespace Online_Text_editor.Controllers
{
    public class DocumentController : Controller
    {
        public IConfiguration configuration;

        public DocumentController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        //get documents available
        public List<DocumentModel> getDocuments()
        {
            List<DocumentModel> documents = new List<DocumentModel>();
            try
            {
                SqlConnection conn = new SqlConnection(configuration.GetConnectionString("textEditorDB"));
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select * from Document";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DocumentModel document = new DocumentModel();
                    document.DocumentId = (int)reader["DocumentId"];
                    document.DocumentName = (string)reader["DocumentName"];
                    document.Title = (string)reader["Title"];
                    document.Content = (string)reader["Content"];
                    document.DateCreated = (DateTime)reader["DateCreated"];
                    document.DateModified = (DateTime)reader["DateModified"];
                    document.UserId = (int)reader["UserId"];
                    documents.Add(document);
                }
            }
            catch(SqlException se)
            {
                Console.WriteLine(se.Message);
            }
            return documents;
        }
        // GET: DocumentController
        public ActionResult ViewDocuments()
        {
            return View(getDocuments());
        }

        //get details of a document
        public DocumentModel getDocument(int id)
        {
            Console.WriteLine("inside get document with id : " + id);
            DocumentModel document = new DocumentModel();

            try
            {
                SqlConnection conn = new SqlConnection(configuration.GetConnectionString("textEditorDB"));
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"select * from Document where DocumentId={id}";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    document.DocumentId = (int)reader["DocumentId"];
                    document.DocumentName = (string)reader["DocumentName"];
                    document.Title = (string)reader["Title"];
                    document.Content = (string)reader["Content"];
                    document.DateCreated = (DateTime)reader["DateCreated"];
                    document.DateModified = (DateTime)reader["DateModified"];
                    document.UserId = (int)reader["UserId"];
                }
            }
            catch (SqlException se)
            {
                Console.WriteLine(se.Message);
            }
            return document;
        }

        // GET: DocumentController/Details/5
        public ActionResult Details(int id)
        {
            return View(getDocument(id));
        }
        [HttpPost]
        public ActionResult Details(int id,string action)
        {
            Console.WriteLine("triggered post function to download a file");
            DownloadFile(id);
            return View(getDocument(id));
        }


        //function to add into database 

        public void AddDocument(DocumentModel document)
        {
            try
            {
                Console.WriteLine("Inside adding a document to database...");
                SqlConnection conn = new SqlConnection(configuration.GetConnectionString("textEditorDB"));
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO Document (DocumentName, Title, Content, DateCreated, DateModified, UserId) " +
                              "VALUES (@DocumentName, @Title, @Content, @DateCreated, @DateModified, @UserId)";

                cmd.Parameters.AddWithValue("@DocumentName", document.DocumentName);
                cmd.Parameters.AddWithValue("@Title", document.Title);
                cmd.Parameters.AddWithValue("@Content", document.Content);
                cmd.Parameters.AddWithValue("@DateCreated", DateTime.Now);
                cmd.Parameters.AddWithValue("@DateModified", DateTime.Now);
                cmd.Parameters.AddWithValue("@UserId", 1);

                int rowsAffected = cmd.ExecuteNonQuery();
                Console.WriteLine($"Added {rowsAffected} rows to the Document table.");
            }
            catch(SqlException se)
            {
                Console.WriteLine(se.Message);
            }
        }
        // GET: DocumentController/Create
        public ActionResult NewDocument()
        {
            return View();
        }

        // POST: DocumentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewDocument(DocumentModel document)
        {
            try
            {
                AddDocument(document);
                return RedirectToAction("ViewDocuments","Document");
            }
            catch
            {
                Console.WriteLine("some error");
                return View();
            }
        }

        // GET: DocumentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View(getDocument(id));
        }


        //update a document function 

        public void UpdateDocument(DocumentModel document,int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("textEditorDB")))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    cmd.CommandText = "UPDATE Document " +
                                      "SET DocumentName = @DocumentName, " +
                                      "    Title = @Title, " +
                                      "    Content = @Content, " +
                                      "    DateModified = @DateModified, " +
                                      "    UserId = @UserId " +
                                      "WHERE DocumentId = @DocumentId";

                    cmd.Parameters.AddWithValue("@DocumentName", document.DocumentName);
                    cmd.Parameters.AddWithValue("@Title", document.Title);
                    cmd.Parameters.AddWithValue("@Content", document.Content);
                    cmd.Parameters.AddWithValue("@DateModified", DateTime.Now);
                    cmd.Parameters.AddWithValue("@UserId", document.UserId);
                    cmd.Parameters.AddWithValue("@DocumentId", id);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"Updated {rowsAffected} rows in the Document table.");
                }
            }
            catch (SqlException se)
            {
                Console.WriteLine(se.Message);
            }
        }

        // POST: DocumentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, DocumentModel document)
        {
            try
            {
                UpdateDocument(document, id);
                return RedirectToAction("ViewDocuments","Document");

            }
            catch
            {
                return View();
            }
        }

        // GET: DocumentController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(getDocument(id));
        }


        // delete a document function
        public void DeleteDocument(int documentId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("textEditorDB")))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    cmd.CommandText = "DELETE FROM Document " +
                                      "WHERE DocumentId = @DocumentId";

                    cmd.Parameters.AddWithValue("@DocumentId", documentId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"Deleted {rowsAffected} rows from the Document table.");
                }
            }
            catch (SqlException se)
            {
                Console.WriteLine(se.Message);
            }
        }

        // POST: DocumentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                DeleteDocument(id);
                return RedirectToAction("ViewDocuments","Document");

            }
            catch
            {
                return View();
            }
        }

        public void DownloadFile(int documentId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("textEditorDB")))
                {
                    Console.WriteLine("trying to download document with id : " + documentId);
                    Console.WriteLine($"trying to save file in path : {Path.GetTempPath()}");
                    conn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT DocumentName, Content FROM Document " +
                                      "WHERE DocumentId = @DocumentId";

                    cmd.Parameters.AddWithValue("@DocumentId", documentId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string fileName = (string)reader["DocumentName"];
                        string fileContentsString = (string)reader["Content"];
                        fileName = fileName.Replace(" ", "_") + ".txt";
                        Console.WriteLine(fileName);
                        Console.WriteLine(fileContentsString);
                        string filePath = Path.Combine(Path.GetFullPath(fileName), fileName);
                        Console.WriteLine(filePath);
                        StreamWriter writer = new StreamWriter(filePath);
                        writer.WriteLine(fileContentsString);
                        writer.Close();
                        //byte[] fileContents = Encoding.UTF8.GetBytes(fileContentsString);

                        // Save file to disk
                        //string filePath = Path.Combine(Path.GetFullPath(fileName), fileName);
                        //File(fileContents,"text");

                        // Download file
                        //FileStream stream = new FileStream(filePath, FileMode.Open);
                        //return File(stream, "application/octet-stream", fileName);
                    }
                    else
                    {
                        //return NotFound();
                    }
                    conn.Close();
                }
            }
            catch (SqlException se)
            {
                Console.WriteLine(se.Message);
                //return StatusCode(500);
            }
        }

    }
}
