using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HierarchyIdWithEfCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {

        [HttpGet]
        public async Task<List<Category>> Get()
        {

            using var dbContext = new CategoryDbContext();
            var result = await dbContext.Categories
                    .OrderBy(x => x.HierarchyId)
                    .ToListAsync();

            return result;
        }

        [HttpPost]
        public async Task<ActionResult> Post()
        {
            using var dbContext = new CategoryDbContext();
            dbContext.Categories.AddRange(
                new Category { Name = "KOZMETİK", HierarchyId = HierarchyId.Parse("/") },
                new Category { Name = "MAKYAJ", HierarchyId = HierarchyId.Parse("/1/") },
                new Category { Name = "CİLT BAKIMI", HierarchyId = HierarchyId.Parse("/2/") },
                new Category { Name = "SAÇ BAKIMI", HierarchyId = HierarchyId.Parse("/3/") },
                new Category { Name = "GÖZ MAKYAJI", HierarchyId = HierarchyId.Parse("/1/1/") },
                new Category { Name = "DUDAK MAKYAJI", HierarchyId = HierarchyId.Parse("/1/2/") },
                new Category { Name = "GÜNEŞ KREMİ", HierarchyId = HierarchyId.Parse("/2/1/") },
                new Category { Name = "CİLT SERUMU", HierarchyId = HierarchyId.Parse("/2/2/") },
                new Category { Name = "ŞAMPUAN", HierarchyId = HierarchyId.Parse("/3/1/") },
                new Category { Name = "SAÇ BOYASI", HierarchyId = HierarchyId.Parse("/3/2/") }
            );
            await dbContext.SaveChangesAsync();

            return Ok();

        }

        [HttpPost("add")]
        public ActionResult PostAdd(string Name, int? parentId)
        {

            using var dbContext = new CategoryDbContext();

            var entity = new Category() { Name = Name };
            dbContext.Categories.Add(entity);
            dbContext.SaveChanges();

            if (!parentId.HasValue)
            {
                entity.HierarchyId = HierarchyId.Parse("/");
                dbContext.SaveChanges();
            }
            else
            {
                var parentEntity = dbContext.Categories.FirstOrDefault(x => x.Id == parentId);

                var lastEntity = dbContext.Categories
                    .Where(x => x.HierarchyId.IsDescendantOf(parentEntity.HierarchyId)
                    && x != parentEntity)
                    .OrderByDescending(x => x.HierarchyId).FirstOrDefault();

                entity.HierarchyId = parentEntity.HierarchyId.GetDescendant(lastEntity?.HierarchyId, null);
                dbContext.SaveChanges();
            }

            return Ok();

        }

        [HttpGet("level/{id}")]
        public async Task<Category> GetLevel(int id)
        { 
            using var dbContext = new CategoryDbContext();
            var category = await dbContext.Categories.FindAsync(id); 
            Console.WriteLine(category.HierarchyId.ToString() + " Level: " + category.HierarchyId.GetLevel()); 
            return category;
        }

        [HttpGet("parent/{id}")]
        public async Task<Category> GetParent(int id)
        {
            using var dbContext = new CategoryDbContext();
            var manager = await dbContext.Categories.FindAsync(id);
            var category = await dbContext.Categories
                    .FirstOrDefaultAsync(e => e.HierarchyId == manager.HierarchyId.GetAncestor(1));
            return category;
        }

        [HttpGet("parents/{id}")]
        public async Task<List<Category>> GetParents(int id)
        {
            using var dbContext = new CategoryDbContext();
            var manager = await dbContext.Categories.FindAsync(id);
            var categories = await dbContext.Categories
                     .Where(e => manager.HierarchyId.IsDescendantOf(e.HierarchyId))
                     .ToListAsync();
            return categories;
        }

        [HttpGet("childs/{id}")]
        public async Task<List<Category>> GetChilds(int id)
        {
            using var dbContext = new CategoryDbContext();
            var manager = await dbContext.Categories.FindAsync(id);
            var categories = await dbContext.Categories
                .Where(x => x.HierarchyId.GetAncestor(1) == manager.HierarchyId)
                .ToListAsync();
            return categories;
        }

        [HttpGet("tree")]
        public async Task<List<Helpers.TreeView>> GetTreeView()
        {
            using var dbContext = new CategoryDbContext();


            var result = await dbContext.Categories
                    .OrderBy(c => c.HierarchyId)
                    .ToListAsync();

            return Helpers.GetTree(result);
        }

    }
}
