using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Pack
{
    public string author;
    public List<Question> favourableOdds = new List<Question>();
    public List<Question> boardGame = new List<Question>();
    public List<Question> thisOrThat = new List<Question>();
}
