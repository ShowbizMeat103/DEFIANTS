using System.Collections.Generic;

namespace DEFIANTS.Shared.DTOs;

public class ErrorResponseDto
{
    public IEnumerable<ErrorDetailDto> Errors { get; set; }
}

public class ErrorDetailDto
{
    public string Code { get; set; }
    public string Description { get; set; }
}
