﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS.Exceptions
{
    class DuplicateEntityException : Exception
    {
        public DuplicateEntityException(EntityPool pool)
            : base($"Two entities in pool {pool.Id} shared the same tag.")
        {

        }
    }
}
