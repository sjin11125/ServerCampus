﻿namespace Com2usServerCampus.Model;

public class Mail
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public int Expire { get; set; }

    public bool isRead { get; set; }
    public bool isGet { get; set; }

    public List<MailItem> Items { get; set; }
}
public class MailItem       //메일에 있는 보상 아이템 리스트
{
    public int Id { get; set; }
    public int Email { get; set; }
    public int Code { get; set; }
    public int Count { get; set; }
}