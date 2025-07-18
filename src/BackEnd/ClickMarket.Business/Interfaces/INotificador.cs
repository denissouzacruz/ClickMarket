﻿using ClickMarket.Business.Notificacoes;

namespace ClickMarket.Business.Interfaces;

public interface INotificador
{
    bool TemNotificacao();
    List<Notificacao> ObterNotificacoes();
    void Handle(Notificacao notificacao);
}
