﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.MsgInterServer
{
   public static class Core
    {

       static bool _IsCrossPkOpen;
       public static bool IsCrossPkOpen
       {
           get
           {
               return _IsCrossPkOpen == true && DateTime.Now < JoinCrossEliteStamp;
           }
           set
           {
               _IsCrossPkOpen = true;
           }
       }
       public static DateTime JoinCrossEliteStamp = new DateTime();
    }
}
