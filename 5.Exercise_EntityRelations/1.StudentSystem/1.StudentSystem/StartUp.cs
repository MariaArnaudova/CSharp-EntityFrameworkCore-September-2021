﻿using P01_StudentSystem.Data;
using System;

namespace _P01_StudentSystem
{
   public class StartUp
    {
       public static void Main(string[] args)
        {
            var context = new StudentSystemContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}