using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Interfaces.Services
{
    public interface IPasswordService
    {
        public void CreatePasswordHash(string password, out byte[] salt, out byte[] hash);
        public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
        public bool CheckPasswordValidation(string password);
    }

}
