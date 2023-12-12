using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext dataContext, IMapper mapper)
        {
            _context = dataContext;
            _mapper= mapper;
        }

        public async Task<MemberDto> GetMemberAsync(string userName)
        {
             return await _context.Users
             .Where(x=>x.UserName == userName)
             .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)  //reason since we need only certain fields as memberdeo why to query all the field of user from database
             .SingleOrDefaultAsync();
             
        }

        public  async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider) //projection is taking care of eager laoding kind of thing
                .ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public  async Task<AppUser> GetUserByUserNameAsync(string userName)
        {
            return await _context.Users.Include(p=>p.Photos)  //eager laoding
            .SingleOrDefaultAsync(x => x.UserName == userName);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.Include(p=>p.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
           return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public   void Update(AppUser user)
        {
           _context.Entry(user).State = EntityState.Modified;
           
        }
    }
}