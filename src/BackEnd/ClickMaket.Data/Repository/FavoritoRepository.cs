﻿using ClickMarket.Business.Interfaces;
using ClickMarket.Business.Models;
using ClickMarket.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ClickMarket.Data.Repository;

public class FavoritoRepository(ClickDbContext context) : Repository<Favorito>(context), IFavoritoRepository
{
}