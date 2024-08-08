using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : SingletonMonoBehaviour<DatabaseManager>
{
    [SerializeField] public MapDatabase mapDatabase;
    [SerializeField] public ItemDatabase itemDatabase;
    [SerializeField] public JobDatabase jobDatabase;

}
