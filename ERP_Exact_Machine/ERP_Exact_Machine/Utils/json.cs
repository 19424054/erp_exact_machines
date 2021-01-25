using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using System.Data;
using System.Collections;

namespace ERP_Exact_Machine.Utils
{
    public class json
    {
        private string dataJson;
        private dynamic data;
        private DataTable dataTable;
        public string error_code = "";
        public string error_message = "";
        public json(string json)
        {
            try
            {
                dataJson = json;
                JObject jsonO = JObject.Parse(dataJson);
                dataJson = jsonO["data"].ToString();


                error_code = jsonO["error_code"].ToString();
                error_message = jsonO["error_message"].ToString();
                if (Config.isEncrypt)
                {
                    dataJson = Utils.Tools.TripleDesDecrypt(dataJson, null);

                }
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    Formatting = Formatting.None,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    FloatParseHandling = FloatParseHandling.Decimal,
                    Converters = new[] { new DataTableConverter() }
                };
                if (dataJson.Equals("{}"))
                {
                    dataTable = null;
                }
                else
                {
                    dataTable = JsonConvert.DeserializeObject<DataTable>(dataJson, settings);
                }
            }
            catch (Exception ex)
            {

            }
        }
        public class DataTableConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (value == null)
                {
                    writer.WriteNull();
                    return;
                }
                DataTable table = (DataTable)value;
                DefaultContractResolver resolver = serializer.ContractResolver as DefaultContractResolver;
                writer.WriteStartArray();
                foreach (DataRow row in table.Rows) {
                    writer.WriteStartObject();
                    foreach (DataColumn column in row.Table.Columns)
                    {
                        object columnValue = row[column];
                        if (serializer.NullValueHandling == NullValueHandling.Ignore && (columnValue == null || columnValue == DBNull.Value))
                        {
                            continue;
                        }
                        writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(column.ColumnName) : column.ColumnName);
                        serializer.Serialize(writer, columnValue);
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) {
                    return null;
                }
                if (!(existingValue is DataTable dt))
                {
                    dt = (objectType == typeof(DataTable)) ? new DataTable()
                    : (DataTable)Activator.CreateInstance(objectType);
                }
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    dt.TableName = (string)reader.Value;
                    reader.Read();
                    if (reader.TokenType == JsonToken.Null)
                    {
                        return dt;
                    }

                }
                if (reader.TokenType == JsonToken.StartArray)
                {
                    return dt;
                }
                reader.Read();
                while (reader.TokenType != JsonToken.EndArray)
                {
                    CreateRow(reader, dt, serializer);
                    reader.Read();
                }
                return dt;
            }
            private static void CreateRow(JsonReader reader, DataTable dt, JsonSerializer serializer)
            {
                DataRow dr = dt.NewRow();
                reader.Read();
                while (reader.TokenType == JsonToken.PropertyName)
                {
                    string columnName = (string)reader.Value;
                    reader.Read();
                    DataColumn column = dt.Columns[columnName];
                    if (column == null)
                    {
                        Type columnType = GetColumnDataType(reader);
                        column = new DataColumn(columnName, columnType);
                        dt.Columns.Add(column);
                    }
                    if (column.DataType == typeof(DataTable))
                    {
                        if (reader.TokenType == JsonToken.StartArray)
                        {
                            reader.Read();
                        }
                        DataTable nestedDt = new DataTable();
                        while (reader.TokenType != JsonToken.EndArray)
                        {
                            CreateRow(reader, nestedDt, serializer);
                            reader.Read();
                        }
                        dr[columnName] = nestedDt;
                    }
                    else if(column.DataType.IsArray && column.DataType != typeof(byte[]))
                    {
                        if (reader.TokenType == JsonToken.StartArray)
                        {
                            reader.Read();
                        }
                        List<object> o = new List<object>();
                        while(reader.TokenType != JsonToken.EndArray)
                        {
                            o.Add(reader.Value);
                            reader.Read();
                        }
                        Array destinationArray = Array.CreateInstance(column.DataType.GetElementType(), o.Count);
                        ((IList)o).CopyTo(destinationArray, 0);
                        dr[columnName] = destinationArray;

                    }
                    else
                    {
                        object columnValue = (reader.Value != null)
                            ? serializer.Deserialize(reader, column.DataType) ?? DBNull.Value
                            : DBNull.Value;
                        dr[columnName] = columnValue;
                    }
                    reader.Read();
                }
                dr.EndEdit();
                dt.Rows.Add(dr);
            }
            private static Type GetColumnDataType(JsonReader reader)
            {
                JsonToken tokenType = reader.TokenType;
                switch (tokenType)
                {
                    case JsonToken.Integer:
                        return typeof(double);
                    case JsonToken.Boolean:
                    case JsonToken.String:
                    case JsonToken.Float:
                    case JsonToken.Date:
                    case JsonToken.Bytes:
                        return reader.ValueType;
                    case JsonToken.Null:
                    case JsonToken.Undefined:
                    case JsonToken.EndArray:
                        return typeof(string);
                    case JsonToken.StartArray:
                        reader.Read();
                        if(reader.TokenType== JsonToken.StartObject)
                        {
                            return typeof(DataTable);
                        }
                        Type arrayType = GetColumnDataType(reader);
                        return arrayType.MakeArrayType();
                    default:
                        throw new JsonSerializationException("Unexpected JSON token when reading DataTable:{0}");
                }
            }
            public override bool CanConvert(Type valueType)
            {
                return typeof(DataTable).IsAssignableFrom(valueType);
            }
        }
        public object getValue(string name, int index = 0)
        {
            object value = null;
            try
            {
                value = dataTable.Rows[index][name];
            }
            catch (Exception ex)
            {

              
            }
            return value;
        }
        public DataTable toDataTable()
        {
            try
            {
                if (dataTable == null)
                {
                    dataTable = JsonConvert.DeserializeObject<DataTable>(dataJson);
                }
            }
            catch (Exception ex)
            {

            }
            return dataTable;
        }
    }
}
