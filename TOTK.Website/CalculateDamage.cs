using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TOTK.Website.Models;
using TOTK.Website.Pages;
using Microsoft.Extensions.Logging;

namespace TOTK.Website
{
    public class CalculateDamage
    {
        private readonly ILogger<CalculateDamage> _logger;
        public CalculateDamage(ILogger<CalculateDamage> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public int Calculate(IndexModel indexModel)
        {
            int DamageOutput;
            byte? BaseAttack = indexModel.SelectedWeapon.BaseAttack;
            DamageOutput = (int)BaseAttack;

            _logger.LogInformation($"{DamageOutput}");
            return DamageOutput;
        }
    }
}
