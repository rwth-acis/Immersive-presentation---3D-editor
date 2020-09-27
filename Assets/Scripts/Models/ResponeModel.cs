using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User
{
    public int iduser { get; set; }
    public string email { get; set; }
}

public class LoginResponse
{
    public User user { get; set; }
    public string token { get; set; }
    public long exp { get; set; }
}
