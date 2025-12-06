using NeoAI;

namespace neodataEcosystem.Data
{

	public class daNeural : dataBaseSuperClass
	{
		public daNeural() : base("neo_neural") { }

		private string ValidateProfile(inNeural _params)
		{
			string _return = "";
			try
			{
				DataTable dtResponse = GetProfile(_params.Id_application, _params.Id_user, _DataContext.DefaultDatabase);
				int id_profile = Convert.ToInt32(dtResponse.Rows[0]["id"]);

				using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
				{
					dtResponse = new DataTable();
					connection.Open();
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = connection;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT * FROM dbo.mod_neural_projects WHERE id=@ID_PROJECT and id_profile=@ID_PROFILE";
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@ID_PROFILE", id_profile);
					cmd.Parameters.AddWithValue("@ID_PROJECT", _params.Id_project);
					dtResponse.Load(cmd.ExecuteReader());
					if (dtResponse.Rows.Count == 0)
					{
						_return = "El [Id_project] no se corresponde con el [Id_profile] del [Id_application ] y [Id_user]";
					}
					else
					{
						if (_params.Id_item != 0 && _params.Id_question != 0)
						{
							cmd.CommandText = "SELECT * FROM dbo.mod_neural_questions WHERE id=@ID_PROJECT";
							cmd.Parameters.Clear();
							cmd.Parameters.AddWithValue("@ID_PROJECT", _params.Id_project);
							dtResponse = new DataTable();
							dtResponse.Load(cmd.ExecuteReader());
							if (dtResponse.Rows.Count == 0)
							{
								_return = "El [Id_question] no se corresponde con el [Id_project] del [Id_profile]";
							}
							else
							{
								cmd.CommandText = "SELECT * FROM dbo.mod_neural_items WHERE id=@ID_PROJECT";
								cmd.Parameters.Clear();
								cmd.Parameters.AddWithValue("@ID_PROJECT", _params.Id_project);
								dtResponse = new DataTable();
								dtResponse.Load(cmd.ExecuteReader());
								if (dtResponse.Rows.Count == 0)
								{
									_return = "El [Id_item] no se corresponde con el [Id_project] del [Id_profile]";
								}
							}
						}
					}
					connection.Close();
				}
				_return = "";
			}
			catch (Exception ex)
			{
				_return = ex.Message;
			}
			return _return;
		}
		private dynamic GetProjectValue<T>(int _id_project, string _field)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
				{
					connection.Open();
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = connection;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT " + _field + " FROM dbo.mod_neural_projects WHERE id=@ID_PROJECT";

					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@ID_PROJECT", _id_project);
					DataTable dtResponse = new DataTable();
					dtResponse.Load(cmd.ExecuteReader());
					if (dtResponse.Rows.Count != 0) { return Tools.ControledCast<T>(dtResponse.Rows[0][_field]); }
					return null;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}
		private dynamic GetQuestionValue<T>(inNeural _params, string _field)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
				{
					connection.Open();
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = connection;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT " + _field + " FROM dbo.mod_neural_questions WHERE id_project=@ID_PROJECT AND id=@ID_QUESTION";

					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@ID_PROJECT", _params.Id_project);
					cmd.Parameters.AddWithValue("@ID_QUESTION", _params.Id_question);
					DataTable dtResponse = new DataTable();
					dtResponse.Load(cmd.ExecuteReader());
					if (dtResponse.Rows.Count != 0) { return Tools.ControledCast<T>(dtResponse.Rows[0][_field]); }
					return null;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}
		private int GeItemFromHash(int _id_proyect, string _hash)
		{
			int _id = 0;
			try
			{
				using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
				{
					connection.Open();
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = connection;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT id FROM dbo.mod_neural_items WHERE id_project=@ID_PROJECT AND hash_data=@HASH";

					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@ID_PROJECT", _id_proyect);
					cmd.Parameters.AddWithValue("@HASH", _hash);
					DataTable dtResponse = new DataTable();
					dtResponse.Load(cmd.ExecuteReader());
					if (dtResponse.Rows.Count != 0) { _id = Convert.ToInt32(dtResponse.Rows[0]["id"].ToString()); }
					return _id;
				}
			}
			catch (Exception)
			{
				return _id;
			}
		}

		public outBaseAnyResponse Resolve(inNeural _params)
		{
			try
			{
				outBaseAnyResponse _response = new outBaseAnyResponse();
				string _validateProfile = ValidateProfile(_params);
				if (_validateProfile != "") { throw new Exception(_validateProfile); }
				int _cols = GetProjectValue<int>(_params.Id_project, "ColumnsMatrix");
				int _lines = GetProjectValue<int>(_params.Id_project, "LinesMatrix");
				int _epochs = GetProjectValue<int>(_params.Id_project, "EpochsMatrix");

				/*Neural net instantiation*/
				NeoMind _neoMind = new NeoMind(_cols, _lines, _epochs);
				double[,] _synapsematrix = new double[_neoMind._lines, _neoMind._columns];
				Byte[] _modelSynapsesMatrix = GetQuestionValue<Byte[]>(_params, "SynapsesMatrix");
				if (_modelSynapsesMatrix == null) { throw new Exception("No se ha efectuado entrenamiento para [Id_project] y [Id_question]"); }
				Tools.FromBytes(_synapsematrix, _modelSynapsesMatrix);

				/*Recovered trainned values to Neural neyt instantiated*/
				_neoMind.UpdateSynapsesMatrix(_synapsematrix, true);

				/*Transform data problem*/
				byte[] _raw_data = System.Convert.FromBase64String(_params.Base64RawData);
				double[,] _data_problem = new double[_neoMind._columns, _neoMind._lines];
				Tools.FromBytes(_data_problem, _raw_data);

				/*Call to instance for resolve problem*/
				_neoMind.ResolveProblem(_data_problem);

				_response.Records = DoubleListToList(_neoMind._response);
				_response.Logic = true;
				_response.Numeric = 0;

				return _response;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		public outBaseAnyResponse SynapsesMatrix(inNeural _params)
		{
			try
			{
				outBaseAnyResponse _response = new outBaseAnyResponse();

				string _validateProfile = ValidateProfile(_params);
				if (_validateProfile != "") { throw new Exception(_validateProfile); }
				int _cols = GetProjectValue<int>(_params.Id_project, "ColumnsMatrix");
				int _lines = GetProjectValue<int>(_params.Id_project, "LinesMatrix");
				int _epochs = GetProjectValue<int>(_params.Id_project, "EpochsMatrix");

				/*Neural net instantiation*/
				NeoMind _neoMind = new NeoMind(_cols, _lines, _epochs);
				double[,] _synapsematrix = new double[_neoMind._lines, _neoMind._columns];
				Byte[] _modelSynapsesMatrix = GetQuestionValue<Byte[]>(_params, "SynapsesMatrix");
				if (_modelSynapsesMatrix != null) { Tools.FromBytes(_synapsematrix, _modelSynapsesMatrix); }

				/*Recovered trainned values to Neural net instantiated*/
				_neoMind.UpdateSynapsesMatrix(_synapsematrix, true);

				/*Acá va la resolucion del problema!*/
				/* ... */
				_response.Records = DoubleListToList(_neoMind._after);
				_response.Logic = true;
				_response.Numeric = 0;

				return _response;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		public outBaseAnyResponse Trainning(inNeural _params)
		{
			try
			{
				outBaseAnyResponse _response = new outBaseAnyResponse();

				string _validateProfile = ValidateProfile(_params);
				if (_validateProfile != "") { throw new Exception(_validateProfile); }
				int _cols = GetProjectValue<int>(_params.Id_project, "ColumnsMatrix");
				int _lines = GetProjectValue<int>(_params.Id_project, "LinesMatrix");
				int _epochs = GetProjectValue<int>(_params.Id_project, "EpochsMatrix");

				/*Neural net instantiation*/
				NeoMind _neoMind = new NeoMind(_cols, _lines, _epochs);

				using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
				{
					connection.Open();
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = connection;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT * FROM dbo.mod_neural_vw_items_for_trainning WHERE id_project=@ID_PROJECT AND id_question=@ID_QUESTION AND trainned IS null";

					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@ID_PROJECT", _params.Id_project);
					cmd.Parameters.AddWithValue("@ID_QUESTION", _params.Id_question);

					DataTable dtResponse = new DataTable();
					dtResponse.Load(cmd.ExecuteReader());

					if (dtResponse.Rows.Count == 0) { throw new Exception("No hay items disponibles para realizar el entrenamiento"); }

					foreach (DataRow drow in dtResponse.Rows)
					{
						double[,] _data_item = new double[_neoMind._columns, _neoMind._lines];
						double[,] _data_response = new double[1, 1];
						Tools.FromBytes(_data_item, (byte[])drow["raw_data_item"]);
						Tools.FromBytes(_data_response, (byte[])drow["raw_data_response"]);
						_neoMind.AddItemForTrainning(_data_item, _data_response);

						/*Update trainned item*/
						cmd.Parameters.Clear();
						cmd.CommandText = "UPDATE mod_neural_item_response SET trainned=GETDATE() WHERE id=@ID_ITEM";
						cmd.Parameters.AddWithValue("@ID_ITEM", drow["id_response"]);
						cmd.ExecuteNonQuery();
					}
					SynapsesMatrix(_params);
					_neoMind.ExecuteTrainning();

					cmd.CommandText = "UPDATE dbo.mod_neural_questions SET SynapsesMatrix=@SYNAPSESMATRIX WHERE id=@ID_QUESTION AND id_project=@ID_PROJECT";
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@ID_PROJECT", _params.Id_project);
					cmd.Parameters.AddWithValue("@ID_QUESTION", _params.Id_question);
					cmd.Parameters.AddWithValue("@SYNAPSESMATRIX", Tools.ToBytes(_neoMind.NN.SynapsesMatrix));
					cmd.ExecuteNonQuery();

					_response.Records = DoubleListToList(_neoMind._after);
					_response.Logic = true;
					_response.Numeric = Convert.ToInt32(dtResponse.Rows.Count);
					connection.Close();

					#region Automatic Log
					//WriteLog(_params.Id_application, _response.Numeric, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
					#endregion
				}
				return _response;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		public outBaseAnyResponse Questions(inNeural _params)
		{
			try
			{
				outAuthentication _response = new outAuthentication(new outBaseAnyResponse());
				using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
				{
					string _validateProfile = ValidateProfile(_params);
					if (_validateProfile != "") { throw new Exception(_validateProfile); }

					connection.Open();
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = connection;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT * FROM dbo.mod_neural_vw_questions WHERE id_project=@ID_PROJECT";
					if (_params.Id_question != 0) { cmd.CommandText += " AND id=@ID_QUESTION"; }

					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@ID_PROJECT", _params.Id_project);
					if (_params.Id_question != 0) { cmd.Parameters.AddWithValue("@ID_QUESTION", _params.Id_question); }

					DataTable dtResponse = new DataTable();
					dtResponse.Load(cmd.ExecuteReader());
					_response.Records = DataTableToList(dtResponse);
					_response.Logic = true;
					_response.Numeric = Convert.ToInt32(_response.Records[0]["id"]);
					connection.Close();

					#region Automatic Log
					//WriteLog(_params.Id_application, _response.Numeric, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
					#endregion
				}
				return _response;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		public outBaseAnyResponse Projects(inNeural _params)
		{
			try
			{
				outAuthentication _response = new outAuthentication(new outBaseAnyResponse());
				using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
				{
					DataTable dtResponse = GetProfile(_params.Id_application, _params.Id_user, _DataContext.DefaultDatabase);
					int id_profile = Convert.ToInt32(dtResponse.Rows[0]["id"]);
					if (dtResponse.Rows.Count == 0) { throw new Exception("No se pudo obtener [Id_profile] con [Id_application ] y [Id_user] provistos"); }

					connection.Open();
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = connection;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT * FROM dbo.mod_neural_projects WHERE id_profile=@ID_PROFILE";
					if (_params.Id_project != 0) { cmd.CommandText += " AND id=@ID_PROJECT"; }
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@ID_PROFILE", id_profile);
					if (_params.Id_project != 0) { cmd.Parameters.AddWithValue("@ID_PROJECT", _params.Id_project); }
					dtResponse = new DataTable();
					dtResponse.Load(cmd.ExecuteReader());
					_response.Records = DataTableToList(dtResponse);
					_response.Logic = true;
					_response.Numeric = Convert.ToInt32(_response.Records[0]["id"]);
					connection.Close();

					#region Automatic Log
					//WriteLog(_params.Id_application, _response.Numeric, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
					#endregion
				}
				return _response;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		public outBaseAnyResponse ResetProject(inNeural _params)
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();
			try
			{
				using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
				{
					string _validateProfile = ValidateProfile(_params);
					if (_validateProfile != "") { throw new Exception(_validateProfile); }

					connection.Open();
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = connection;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "DELETE dbo.mod_neural_item_response WHERE id_question IN (SELECT id FROM DBO.mod_neural_questions WHERE id_project=@ID_PROJECT)";
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@ID_PROJECT", _params.Id_project);
					_response.Numeric = Convert.ToInt32(cmd.ExecuteNonQuery());

					cmd.CommandText = "DELETE dbo.mod_neural_items WHERE id_project=@ID_PROJECT";
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@ID_PROJECT", _params.Id_project);
					_response.Numeric += Convert.ToInt32(cmd.ExecuteNonQuery());

					cmd.CommandText = "UPDATE dbo.mod_neural_questions SET SynapsesMatrix=null WHERE id_project=@ID_PROJECT";
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@ID_PROJECT", _params.Id_project);
					_response.Numeric += Convert.ToInt32(cmd.ExecuteNonQuery());

					connection.Close();

					#region Automatic Log
					WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
					#endregion
				}
				return _response;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		public outBaseAnyResponse AddItemforTranning(inNeural _params)
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();
			try
			{
				/*Transforma el base64 con los datos a un array de bytes*/
				byte[] _raw_data = System.Convert.FromBase64String(_params.Base64RawData);
				Console.WriteLine(_params.Base64RawData);
				string _hashed_raw_data = _params.Base64RawData;
				int _id = 0;// GeItemFromHash(_params.Id_project, _hashed_raw_data);

				using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
				{
					string _validateProfile = ValidateProfile(_params);
					if (_validateProfile != "") { throw new Exception(_validateProfile); }

					connection.Open();
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = connection;
					cmd.CommandType = CommandType.Text;
					if (_id != 0)
					{
						// Response with matched item already loaded!
						_response.Numeric = _id;
					}
					else
					{
						cmd.CommandText = "INSERT INTO dbo.mod_neural_items ";
						cmd.CommandText += " (" + _DataContext.getCommonFields() + ",id_project,raw_data,hash_data) ";
						cmd.CommandText += " VALUES ";
						cmd.CommandText += " (" + _DataContext.getCommonFieldsValues("", "Item for trainning") + ",@ID_PROJECT,@RAW_DATA,@HASH_DATA) ";
						cmd.CommandText += " ; SELECT SCOPE_IDENTITY()";
						cmd.Parameters.Clear();
						cmd.Parameters.AddWithValue("@ID_PROJECT", _params.Id_project);
						cmd.Parameters.AddWithValue("@RAW_DATA", _raw_data);
						cmd.Parameters.AddWithValue("@HASH_DATA", _hashed_raw_data);
						_response.Numeric = Convert.ToInt32(cmd.ExecuteScalar());
						if (_response.Numeric == 0) { throw new Exception("No id", new Exception("901.02")); }
						connection.Close();
					}

					#region Automatic Log
					//WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
					#endregion
				}
				return _response;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		public outBaseAnyResponse AddItemResponseforTranning(inNeural _params)
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();
			try
			{
				using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
				{
					string _validateProfile = ValidateProfile(_params);
					if (_validateProfile != "") { throw new Exception(_validateProfile); }

					Byte[] _bData = System.Convert.FromBase64String(_params.Base64RawData);

					connection.Open();
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = connection;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "INSERT INTO dbo.mod_neural_item_response ";
					cmd.CommandText += " (" + _DataContext.getCommonFields() + ",id_item,raw_data,id_question) ";
					cmd.CommandText += " VALUES ";
					cmd.CommandText += " (" + _DataContext.getCommonFieldsValues("", "Response expected") + ",@ID_ITEM,@RAW_DATA,@ID_QUESTION) ";
					cmd.CommandText += " ; SELECT SCOPE_IDENTITY()";
					cmd.Parameters.Clear();
					/*Transforma el base64 con los datos a un array de bytes*/
					cmd.Parameters.AddWithValue("@ID_ITEM", _params.Id_item);
					cmd.Parameters.AddWithValue("@RAW_DATA", _bData);
					cmd.Parameters.AddWithValue("@ID_QUESTION", _params.Id_question);
					_response.Numeric = Convert.ToInt32(cmd.ExecuteScalar());
					if (_response.Numeric == 0) { throw new Exception("No id", new Exception("901.02")); }
					connection.Close();

					#region Automatic Log
					//WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
					#endregion
				}
				return _response;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		public outBaseAnyResponse State(inBaseAnyRequest _params)
		{
			outAuthentication _response = new outAuthentication(new outBaseAnyResponse());
			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				DataTable dtResponse = new DataTable();
				_response.Records = DataTableToList(dtResponse);
				_response.Logic = true;
				_response.Numeric = 0;
				_response.Message = "La plataforma está operativa y sus credenciales válidas.";
				_response.Trace = "NeoNeural";
				_response.Scope = "V1";
			}
			return _response;
		}
	}
}
