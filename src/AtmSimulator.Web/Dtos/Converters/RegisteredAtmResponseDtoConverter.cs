using AtmSimulator.Web.Models.Domain;

namespace AtmSimulator.Web.Dtos
{
    public static class RegisteredAtmResponseDtoConverter
    {
        public static RegisteredAtmResponseDto ToDto(this Atm atm)
            => new RegisteredAtmResponseDto
            {
                AtmId = atm.Id,
            };
    }
}
