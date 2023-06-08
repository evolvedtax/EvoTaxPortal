using AutoMapper;
using Azure;
using EvolvedTax.Data.Models.DTOs.Response;
using EvolvedTax.Data.Models.Entities;
using System.Reflection;

namespace EvolvedTax.Helpers
{
    public class AutoMapperProfileConfig : Profile
    {
        public AutoMapperProfileConfig()
        {
            //var dbEntitiesNameSpace = new string[] { "EvolvedTax.Data.Models.Entities" };
            //var dtoNameSpace = new string[] { "EvolvedTax.Data.Models.DTOs.Response" };
            //var assembly = Assembly.GetExecutingAssembly();
            //var dbEntities = GetTypesInNamespace(assembly, dbEntitiesNameSpace).ToList();
            //var rerquest = GetTypesInNamespace(assembly, dtoNameSpace).ToList();
            //foreach (var dbEntity in dbEntities)
            //{
            //    Type response = rerquest.FirstOrDefault(p => p.Name == dbEntity.Name + "Response");
            //    if (response != null)
            //    {
            //        CreateMap(dbEntity, response).ReverseMap();
            //    }
            //}
            CreateMap(typeof(InstituteMasterResponse),typeof(InstituteMaster)).ReverseMap();
            CreateMap(typeof(InstituteEntitiesResponse),typeof(InstituteEntity)).ReverseMap();
            CreateMap(typeof(InstituteClientResponse),typeof(InstitutesClient)).ReverseMap();
        }

        private static List<Type> GetTypesInNamespace(Assembly assembly, string[] dtoNameSpace)
        {
            var types = new List<Type>();
            foreach (var nameSpace in dtoNameSpace)
            {
                types.AddRange(assembly.GetTypes().Where(p =>
                {
                    var da = string.Equals(p.Namespace, nameSpace, StringComparison.Ordinal) && p.IsClass && p.IsPublic;
                    return da;
                }).ToArray());
            }
            return types;
        }
    }
}
