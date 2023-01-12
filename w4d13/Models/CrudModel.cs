using System.Data;
using System.Data.SqlClient;
using w4d13.Data;
using System.Reflection;
using System.Diagnostics;

namespace w4d13.Models
{
    //todo: verify three find functions and add view to it. 
    public class CrudModel
    {
        //private readonly string _connectionString;

        private SqlConnection connection;

        private SqlDataAdapter adapter;

        private readonly IConfiguration _configuration;

        public CrudModel(IConfiguration configuration)
        {
            _configuration = configuration;
            this.connection = new SqlConnection(this._configuration.GetConnectionString("MyConn"));
            this.adapter = new SqlDataAdapter();
        }

        /*
         * Update + Insert combined utility, can be directly used for
         * - AddStudent
         * - AddCourse
         * - AddProfessor
         * - UpdateStudent
         * - UpdateCourse
         * - UpdateProfessor
         * Should be able to help the following functions with small tweaks
         * - AssignStudentToCourse
         * - AssignProfessorToCourse
         * Not useful for
         * - Find**
         */
        public DataTable UpsertWithSelectCmd(string cmd, object from)
        {
            // debug hook
            //cmd = "select 1";

            Debug.Assert(cmd.StartsWith("select", StringComparison.OrdinalIgnoreCase));

            adapter = new SqlDataAdapter();
            adapter.SelectCommand = new SqlCommand(cmd, connection);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

            connection.Open();

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            bool found = false;
            // extract id from object
            string objId = "objId";
            foreach (PropertyInfo property in from.GetType().GetProperties())
            {
                if (property.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    objId = property.GetValue(from)?.ToString() ?? "Unexpected Null";
                }
            }

            // look up the datatable, attempt to find a matching id
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                string dbId = dr["id"]?.ToString() ?? "Expected column id not found";
                if (dbId == objId)
                {
                    // found the row with matching id, so we update the row
                    found = true;
                    foreach (PropertyInfo property in from.GetType().GetProperties())
                    {
                        if (property.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        dr[property.Name.ToLower()] = property.GetValue(from);
                    }
                }
            }

            if (!found)
            {
                // insert block
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo property in from.GetType().GetProperties())
                {
                    if (property.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    dr[property.Name.ToLower()] = property.GetValue(from);
                }

                dt.Rows.Add(dr);
            }

            adapter.Update(dt);
            return dt;
        }

        public DataTable ExecuteSqlCommand(string cmdStr)
        {
            DataTable dt = new DataTable();
            connection.Open();
            SqlCommand cmd = new SqlCommand(cmdStr, connection);
            if (this.adapter != null)
                this.adapter.Dispose();
            adapter = new SqlDataAdapter(cmd);
            if (adapter != null)
            {
                // get
                adapter.Fill(dt);
            }

            return dt;
        }

        public DataTable InsertWithSelectCmd(string cmd, object from)
        {
            Debug.Assert(cmd.StartsWith("select", StringComparison.OrdinalIgnoreCase));
            adapter = new SqlDataAdapter();
            adapter.SelectCommand = new SqlCommand(cmd, connection);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

            connection.Open();

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            {
                // insert block
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo property in from.GetType().GetProperties())
                {
                    if (property.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    dr[property.Name.ToLower()] = property.GetValue(from);
                }

                dt.Rows.Add(dr);
            }

            adapter.Update(dt);
            return dt;
        }
        public DataTable AssignStudentToCourse(int studentId, int courseId)
        {
            string cmd = "SELECT * FROM student_course";
            //Debug.Assert(cmd.StartsWith("select", StringComparison.OrdinalIgnoreCase));
            adapter = new SqlDataAdapter() ;
            adapter.SelectCommand = new SqlCommand(cmd, connection);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter) ;
            connection.Open();
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            DataRow dr = dt.NewRow();
            dr["studentId"] = studentId;
            dr["courseId"] = courseId;
            dt.Rows.Add(dr);
            adapter.Update(dt);
            return dt;
        }

        public DataTable AssignProfessorToCourse(int profId, int courseId)
        {
            string cmd = "SELECT * FROM course WHERE Id =@id";
            //Debug.Assert(cmd.StartsWith("select", StringComparison.OrdinalIgnoreCase));
            adapter = new SqlDataAdapter();
            adapter.SelectCommand = new SqlCommand(cmd, connection);
            adapter.SelectCommand.Parameters.AddWithValue("@id", courseId);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            connection.Open();
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            DataRow dr = dt.Rows[0];
            dr["professorId"] = profId;
            adapter.Update(dt);
            return dt;
        }
        public DataTable GetAllStudents()
        {
            return ExecuteSqlCommand("Select * from Student");
        }

        public DataTable GetAllCourses()
        {
            return ExecuteSqlCommand("Select * from Course");
        }

        public DataTable GetAllProfessors()
        {
            return ExecuteSqlCommand("Select * from Professor");
        }

        public DataTable GetStudentToCourse()
        {
            return ExecuteSqlCommand("Select * from Student_Course");
        }

        public List<Course> FindStudentCourse(string email)
        {
            string findByEmail = "SELECT c.* FROM Course c INNER JOIN Student_Course sc ON c.Id = sc.CourseId INNER JOIN Student s ON s.Id = sc.StudentId WHERE s.Email = @email"; ;

            using (var conn= connection)
            {
                conn.Open();
                var courses = new List<Course>();
                var cmd = new SqlCommand(findByEmail, conn);
                cmd.Parameters.AddWithValue("@email",email);
                using(var reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        courses.Add(new Course
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            ProfessorId = reader.GetInt32(reader.GetOrdinal("ProfessorId"))
                        });
                    }
                }

                    return courses;
                }
            }
            



            // legacy



            //public void AddStudent(Student student)
            //{
            //    //var res = ExecuteSqlCommand(
            //    //    $"INSERT INTO Student VALUES ('{student.FirstName}', '{student.LastName}', '{student.Email}')");

            //    UpsertWithSelectCmd("SELECT * FROM Student", student);

            //    return;
            //}
            ////public void AddCourse(Course course)
            ////{
            ////    var res = ExecuteSqlCommand(
            ////        $"INSERT INTO Course VALUES ('{course.Name}', '{course.Description}', {course.ProfessorId})");
            ////    return;
            ////}

            //public void AddProfessor(Professor prof)
            //{
            //    var res = ExecuteSqlCommand(
            //        $"INSERT INTO Professor VALUES ('{prof.FirstName}', '{prof.LastName}', '{prof.Email}', '{prof.Office}', '{prof.Title}')");
            //    return;
            //}

            //public void AssignStudentToCourse(int studentId, int courseId)
            //{
            //    DataTable studentToCourses = GetStudentToCourse();
            //    bool existing = false;
            //    for (int i = 0; i < studentToCourses.Rows.Count; i++)
            //    {
            //        DataRow row = studentToCourses.Rows[i];
            //        if (row["studentId"].ToString() == studentId.ToString() && row["courseId"].ToString() == courseId.ToString())
            //            existing = true;
            //    }

            //    if(!existing)
            //    {
            //        var newRow = studentToCourses.NewRow();
            //        newRow["studentId"] = studentId;
            //        newRow["courseId"] = courseId;
            //    }

            //    adapter.Update(studentToCourses);
            //    adapter.Dispose();
            //}

            //public void AssignProfessorToCourse(int profId, int courseId)
            //{
            //    DataTable courses = GetAllCourses();
            //    for(int i = 0; i < courses.Rows.Count; i++)
            //    {
            //        DataRow row = courses.Rows[i];
            //        if (row["Id"].ToString() == courseId.ToString())
            //        {
            //            courses.Rows[i]["professorId"] = profId;
            //        }
            //    }

            //    adapter.Update(courses);
            //    adapter.Dispose();
            //}

            //public void UpdateStudent(Student student)
            //{
            //    DataTable students = GetStudentToCourse();
            //    bool existing = false;
            //    for (int i = 0; i < students.Rows.Count; i++)
            //    {
            //        DataRow row = students.Rows[i];
            //        if (row["firstname"].ToString() == student.FirstName
            //            && row["lastname"].ToString() == student.LastName
            //            && row["email"].ToString() == student.Email)
            //            existing = true;
            //    }

            //    if (!existing)
            //    {
            //        var newRow = students.NewRow();
            //        newRow["firstname"] = student.FirstName;
            //        newRow["lastname"] = student.LastName;
            //        newRow["email"] = student.Email;
            //    }

            //    adapter.Update(students);
            //    adapter.Dispose();
            //}

            //public void UpdateCourse(Course course)
            //{
            //    DataTable courses = GetAllCourses();
            //    bool existing = false;
            //    for (int i = 0; i < courses.Rows.Count; i++)
            //    {
            //        DataRow row = courses.Rows[i];
            //        if (row["name"].ToString() == course.Name
            //            && row["description"].ToString() == course.Description
            //            && row["professorId"].ToString() == course.ProfessorId.ToString())
            //            existing = true;
            //    }

            //    if (!existing)
            //    {
            //        var newRow = courses.NewRow();
            //        newRow["name"] = course.Name;
            //        newRow["description"] = course.Description;
            //        newRow["professorId"] = course.ProfessorId;  
            //    }

            //    adapter.Update(courses);
            //    adapter.Dispose();
            //}

            //public void UpdateProfessor(Professor professor)
            //{
            //    DataTable professors = GetAllProfessors();
            //    bool existing = false;
            //    for (int i = 0; i < professors.Rows.Count; i++)
            //    {
            //        DataRow row = professors.Rows[i];
            //        if (row["firstname"].ToString() == professor.FirstName
            //            && row["lastname"].ToString() == professor.LastName
            //            && row["email"].ToString() == professor.Email
            //            && row["office"].ToString() == professor.Office
            //            && row["title"].ToString() == professor.Title)
            //            existing = true;
            //    }

            //    if (!existing)
            //    {
            //        var newRow = professors.NewRow();
            //        newRow["firstname"] = professor.FirstName;
            //        newRow["lastname"] = professor.LastName;
            //        newRow["email"] = professor.Email;
            //        newRow["office"] = professor.Office;
            //        newRow["title"] = professor.Title;
            //    }

            //    adapter.Update(professors);
            //    adapter.Dispose();
            //}

            //public List<Course> FindStudentCoursesByEmail(string email)
            //{
            //    var students = GetAllStudents();
            //    var courses = GetAllCourses();
            //    this.adapter.Dispose();
            //    HashSet<string> courseIds = new HashSet<string>();
            //    for (int i = 0; i < students.Rows.Count; i++)
            //    {
            //        DataRow row = students.Rows[i];
            //        if (row["email"].ToString() == email)
            //        {
            //            courseIds.Add(row["courseId"].ToString());
            //        }
            //    }

            //    List<Course> result = new List<Course>();
            //    for (int i = 0; i < courses.Rows.Count; i++)
            //    {
            //        DataRow row = courses.Rows[i];
            //        if (courseIds.Contains(row["id"]))
            //        {
            //            var c = new Course();
            //            c.Id = Int32.Parse(row["id"].ToString());
            //            c.Name = row["name"].ToString();
            //            c.Description = row["description"].ToString();
            //            c.ProfessorId = Int32.Parse(row["professorId"].ToString());
            //            result.Add(new Course());
            //        }
            //    }

            //    return result;
            //}

            //public List<Course> FindProfessorCoursesByName(string firstName, string lastName)
            //{
            //    var professors = GetAllStudents();
            //    var courses = GetAllCourses();
            //    this.adapter.Dispose();
            //    HashSet<string> professorIds = new HashSet<string>();
            //    for (int i = 0; i < professors.Rows.Count; i++)
            //    {
            //        DataRow row = professors.Rows[i];
            //        if (row["FirstName"].ToString() == firstName && row["LastName"].ToString() == lastName)
            //        {
            //            professorIds.Add(row["id"].ToString());
            //        }
            //    }

            //    List<Course> result = new List<Course>();
            //    for (int i = 0; i < courses.Rows.Count; i++)
            //    {
            //        DataRow row = courses.Rows[i];
            //        if (professorIds.Contains(row["professorId"]))
            //        {
            //            var c = new Course();
            //            c.Id = Int32.Parse(row["id"].ToString());
            //            c.Name = row["name"].ToString();
            //            c.Description = row["description"].ToString();
            //            c.ProfessorId = Int32.Parse(row["professorId"].ToString());
            //            result.Add(new Course());
            //        }
            //    }

            //    return result;
            //}

            //public void FindCourseById(int courseId, out Course c, out Professor p)
            //{
            //    var professors = GetAllStudents();
            //    var courses = GetAllCourses();
            //    this.adapter.Dispose();
            //    string profId = "";
            //    c = new Course();
            //    p = new Professor();
            //    for (int i = 0; i < courses.Rows.Count; i++)
            //    {
            //        DataRow row = courses.Rows[i];
            //        if (row["id"].ToString() == courseId.ToString())
            //        {
            //            profId = row["professorId"].ToString();

            //            c.Id = Int32.Parse(row["id"].ToString());
            //            c.Name = row["name"].ToString();
            //            c.Description = row["description"].ToString();
            //            c.ProfessorId = Int32.Parse(row["professorId"].ToString());
            //        }
            //    }

            //    for (int i = 0; i < professors.Rows.Count; i++)
            //    {
            //        DataRow row = courses.Rows[i];
            //        if (row["id"].ToString() == profId)
            //        {
            //            profId = row["professorId"].ToString();
            //            p.Id = Int32.Parse(row["professorId"].ToString());
            //            p.FirstName = row["firstname"].ToString();
            //            p.LastName = row["lastname"].ToString();
            //            p.Email = row["email"].ToString();
            //            p.Office = row["office"].ToString();
            //            p.Title = row["title"].ToString();
            //        }
            //    }
            //}


        }
}