namespace EasyOC.OrchardCore.RDBMS.Services.Dto;

public record GenerateRecipeDto(
    string ConnectionConfigId,
    string TableName,
    string RecipeContent,
    string TypeName
);
