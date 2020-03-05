﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Databases;
using Entities;
using Npgsql;
using Postgresql.Services;
using NpgsqlTypes;

namespace Postgresql.Services.DatabaseServices
{
    public class NoticeServices_Job : StateConnection,IServices<JobNoticeDB>
    {
        private GraduateServices graduateservice = new GraduateServices();
        private JobServices jobservice = new JobServices();
        private SubMethods method = new SubMethods();
        public Entities.EntityResult<JobNoticeDB> getList()
        {
            EntityResult<JobNoticeDB> result = new EntityResult<JobNoticeDB>();
            result.Result = true;
            result.ErrorText = "Sucsess";
            try
            {
                List<JobNoticeDB> noticeList = new List<JobNoticeDB>();
                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM \"Notice\" WHERE \"isIntern\"=FALSE;", connectionOpen()); //connectionOpen connection döndürüyor
                //id   --  graduateid--  title -- text -- date -- workfield--intershipperiod--intershiplenght--jobsalary---isIntern
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    
                    JobNoticeDB notice = new JobNoticeDB();
                    notice.notice_id = reader.GetInt32(0);

                    //notice.notice_graduate = jobservice.getId(reader.GetInt32(1));
                    notice.notice_graduate.graduate_id = reader.GetInt32(1);
                    notice.notice_graduate = graduateservice.getId(notice.notice_graduate.graduate_id).Object;
                    notice.notice_title = reader.GetString(2);
                    notice.notice_text = reader.GetString(3);

                    NpgsqlDate date1 = reader.GetDate(4);
                    DateTime date2 = new DateTime(date1.Year, date1.Month, date1.Day);
                    notice.notice_date = date2;

                    notice.notice_workfield = reader.GetString(5);
                    notice.notice_job_salary = reader.GetInt32(8);
                    notice.isIntern = reader.GetBoolean(9);


                    noticeList.Add(notice);
                }
                result.Objects = noticeList; //birden fazla liste dönülüyor
                connectionClosed();

            }
            catch (NpgsqlException Ex)//npgsqlexception pgadminden dönen erroru döner
            {

                result.Result = false;
                result.ErrorText = Ex.Message;

            }

            connectionClosed();
            return result;
        }
        public Entities.EntityResult<JobNoticeDB> Insert(JobNoticeDB entities)
        {
            EntityResult<JobNoticeDB> result = new EntityResult<JobNoticeDB>();
            result.Result = true;
            result.ErrorText = "Sucsess";
            try
            {

                NpgsqlCommand command = new NpgsqlCommand("INSERT INTO \"Notice\" (notice_id,\"notice_graduate_id_FK\",notice_title,notice_text,notice_date,notice_workfield,notice_job_salary,\"isIntern\")" +
                    " VALUES(@id,@grad_fk,@title,@text,@date,@workfield,@salary,@is)", connectionOpen()); //connectionOpen connection döndürüyor
                command.Parameters.AddWithValue("@id", entities.notice_id);
                command.Parameters.AddWithValue("@grad_fk", entities.notice_graduate.graduate_id);
                command.Parameters.AddWithValue("@title", entities.notice_title);
                command.Parameters.AddWithValue("@text", entities.notice_text);
                NpgsqlDateTime date = NpgsqlDateTime.Now;
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@salary", entities.notice_job_salary);
                command.Parameters.AddWithValue("@workfield", entities.notice_workfield);
                command.Parameters.AddWithValue("@is", false);

                command.ExecuteNonQuery();

                connectionClosed();
            }
            catch (NpgsqlException Ex)//npgsqlexception pgadminden dönen erroru döner
            {
                result.Result = false;
                result.ErrorText = Ex.Message;
            }
            connectionClosed();
            return result;
        }

        public Entities.EntityResult<JobNoticeDB> Delete(int id)
        {
            EntityResult<JobNoticeDB> result = new EntityResult<JobNoticeDB>();
            result.Result = true;
            result.ErrorText = "Sucsess";
            try
            {
                method.deleteNotice(id);
            }
            catch (NpgsqlException Ex)//npgsqlexception pgadminden dönen erroru döner
            {
                result.Result = false;
                result.ErrorText = Ex.Message;
            }
            connectionClosed();
            return result;

        }

        public Entities.EntityResult<JobNoticeDB> getId(int id)
        {
            EntityResult<JobNoticeDB> result = new EntityResult<JobNoticeDB>();
            result.Result = true;
            result.ErrorText = "Sucsess";
            try
            {
                JobNoticeDB notice = new JobNoticeDB();
                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM \"Notice\" WHERE notice_id=" + id + ";", connectionOpen());
                //id   --  graduateid--  title -- text -- date -- workfield--intershipperiod--intershiplenght--jobsalary---isIntern
                NpgsqlDataReader reader = command.ExecuteReader();

                reader.Read();
                notice.notice_id = reader.GetInt32(0);

                //notice.notice_graduate = jobservice.getId(reader.GetInt32(1));
                notice.notice_graduate.graduate_id = reader.GetInt32(1);
                notice.notice_graduate = graduateservice.getId(notice.notice_graduate.graduate_id).Object;
                notice.notice_title = reader.GetString(2);
                notice.notice_text = reader.GetString(3);

                NpgsqlDate date1 = reader.GetDate(4);
                DateTime date2 = new DateTime(date1.Year, date1.Month, date1.Day);
                notice.notice_date = date2;

                notice.notice_workfield = reader.GetString(5);
                notice.notice_job_salary = reader.GetInt32(8);
                notice.isIntern = reader.GetBoolean(9);
                result.Object = notice;
                connectionClosed();

            }
            catch (NpgsqlException Ex)//npgsqlexception pgadminden dönen erroru döner
            {
                result.Result = false;
                result.ErrorText = Ex.Message;
            }
            connectionClosed();
            return result;
        }

        public Entities.EntityResult<JobNoticeDB> Update(JobNoticeDB entities)
        {
            EntityResult<JobNoticeDB> result = new EntityResult<JobNoticeDB>();



            return result;
        }
        public int getNextPersonId()
        {
            NpgsqlCommand commands = new NpgsqlCommand("SELECT MAX(notice_id) FROM \"Notice\"", connectionOpen()); //connectionOpen connection döndürüyor

            commands.ExecuteNonQuery();
            NpgsqlDataReader reader = commands.ExecuteReader();

            reader.Read();

            var pin = reader.GetInt32(0);
            connectionClosed();
            return pin + 1;
        }
        public EntityResult<JobNoticeDB> UpdateSelected(string table, string colum, int value, int id)
        {
            EntityResult<JobNoticeDB> result = new EntityResult<JobNoticeDB>();
            result.Result = true;
            result.ErrorText = "Sucsess";

            try
            {
                NpgsqlCommand command = new NpgsqlCommand("UPDATE @table SET @col = @value  WHERE @tablename = @id; ", connectionOpen()); //connectionOpen connection döndürüyor
                command.Parameters.AddWithValue("@table", "\"" + table + "\"");
                command.Parameters.AddWithValue("@col", colum);
                command.Parameters.AddWithValue("@value", value);
                command.Parameters.AddWithValue("@id", id) ;
                command.Parameters.AddWithValue("@tablename", table.ToLower() + "_id");

                command.ExecuteNonQuery();
                connectionClosed();
            }
            catch (NpgsqlException Ex)//npgsqlexception pgadminden dönen erroru döner
            {
                result.Result = false;
                result.ErrorText = Ex.Message;
            }
            connectionClosed();
            return result;
        }
    }
}
