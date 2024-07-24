using System.ComponentModel.DataAnnotations;
public class Register
{
    public string name { get; set; }
    public string position { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public string UserType { get; set; }
    public int user_id { get; set; }
    public string status { get; set; }
}