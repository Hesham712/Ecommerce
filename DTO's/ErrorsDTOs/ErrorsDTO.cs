namespace Ecommerce.DTO_s.ErrorsDTOs
{
    public class ErrorsDTO
    {
        public bool IsValid { get; set; }

        public IList<ErrorDTO> Errors { get; set; } = new List<ErrorDTO>();
    }
}
