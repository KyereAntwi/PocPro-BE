namespace DevSync.PocPro.Identity.Pages.Admin.Clients;

[SecurityHeaders]
[Authorize]
public class NewModel : PageModel
{
    private readonly ClientRepository _repository;

    public NewModel(ClientRepository repository)
    {
        _repository = repository;
    }

    [BindProperty] public CreateClientModel InputModel { get; set; }

    public bool Created { get; set; }

    public void OnGet()
    {
        InputModel = new CreateClientModel
        {
            Secret = Convert.ToBase64String(CryptoRandom.CreateRandomKey(16))
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            await _repository.CreateAsync(InputModel);
            Created = true;
        }

        return Page();
    }
}