using JadaraITKnowledgeSystem.Application.Fetures.Majors.Dtos;
using JadaraITKnowledgeSystem.Domain.Universities.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Majors.Mappers
{
    public static class MajorMapper
    {
        public static MajorDto ToDto(this Major major)
        {
            ArgumentNullException.ThrowIfNull(major);

            return new MajorDto
            {
                Id = major.Id,
                Name = major.Name,
                FacultyId = major.FacultyId
            };
        }

        public static List<MajorDto> ToDtos(this IEnumerable<Major> majors)
        {
            return majors.Select(major => major.ToDto()).ToList();
        }

        public static Major ToEntity(this MajorDto majorDto)
        {
            ArgumentNullException.ThrowIfNull(majorDto);

            return Major.Create(majorDto.Name, majorDto.FacultyId).Value;
        }

        public static List<Major> ToEntities(this IEnumerable<MajorDto> majorDtos)
        {
            return majorDtos.Select(majorDto => majorDto.ToEntity()).ToList();
        }
    }
}
