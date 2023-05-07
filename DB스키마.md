

# Account DB
  
## account 테이블
게임에서 생성 된 계정 정보들을 가지고 있는 테이블    
  
```sql

CREATE TABLE IF NOT EXISTS account_db.`account`
(
    AccountId BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY COMMENT '계정번호',
    Email VARCHAR(50) NOT NULL UNIQUE COMMENT '이메일',
    HashedPassword VARCHAR(100) NOT NULL COMMENT '해싱된 비밀번호',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '생성 날짜'
) ;
```   
   
<br>  
<br>  
   
   
# Game DB
  ## gamedata 테이블
유저 게임 데이터 테이블
```sql
  CREATE TABLE IF NOT EXISTS gamedata_db.`gamedata`
(

    Email VARCHAR(50) NOT NULL primary key COMMENT '이메일',
    Exp int NOT NULL COMMENT  '경험치',
    Attack int NOT NULL COMMENT '공격력',
    Defense int Not Null comment '방어력',
    Attendance datetime not null COMMENT '마지막 출석 날짜',
    AttendanceCount int not null comment '출석 카운트'
    
);
```

  ## item 테이블
유저 아이템 테이블
```sql
  CREATE TABLE IF NOT EXISTS gamedata_db.`itemdata`
(

    Email VARCHAR(50) NOT NULL COMMENT '이메일',
    ItemCode int NOT NULL COMMENT  '아이템코드',
    EnhanceCount int NOT NULL COMMENT '강화 횟수',
    ItemCount int NOT NULL COMMENT '아이템 갯수'
);
```

## mail 테이블
유저 메일 테이블
```sql
CREATE TABLE IF NOT EXISTS gamedata_db.`mail`
(
    Id bigint not null auto_increment primary key comment '메일번호',
    Email VARCHAR(50) NOT NULL COMMENT '이메일',
    Title VARCHAR(100) NOT NULL COMMENT  '제목',
    Content VARCHAR(100) NOT NULL COMMENT '내용',
    isRead bool NOT NULL COMMENT '읽었는지 유무',
    isGet bool NOT NULL COMMENT '아이템 받았는지 유무',
    Time datetime default current_timestamp comment '발송날짜',
    ExpiryTime int comment '유효기간'
);

```
  ## mailitem 테이블
유저 메일의 아이템 테이블
```sql
  CREATE TABLE IF NOT EXISTS gamedata_db.`mailitem`
(
    Id bigint not null comment '메일번호',
    Email VARCHAR(50) NOT NULL COMMENT '이메일',
    Code int NOT NULL COMMENT '아이템 코드',
    Count int NOT NULL COMMENT '아이템 갯수'
);
```

   ## inapppurchasereceipt 테이블
인앱결제 영수증 테이블(누가 어떤 상품을 언제 샀다)
```sql
DROP TABLE IF EXISTS gamedata_db.`inapppurchasereceipt`;
  CREATE TABLE IF NOT EXISTS gamedata_db.`inapppurchasereceipt`
(
    Id VARCHAR(50) not null  primary key comment '영수증id',
    Email VARCHAR(50) NOT NULL COMMENT '이메일',
    Title VARCHAR(100) NOT NULL COMMENT  '제목',
    Code int NOT NULL COMMENT '상품 코드',
    Content VARCHAR(100) NOT NULL COMMENT '내용',
    Time datetime Not Null comment '발송날짜',
    ExpiryTime int comment '유효기간'
);
```
   ## inapppurchaseitem 테이블
인앱결제 아이템 테이블(누가 어떤 상품을 샀는데 그 상품이 뭔지!!!!!!!!!!!!)
```sql
DROP TABLE IF EXISTS gamedata_db.`inapppurchaseitem`;

  CREATE TABLE IF NOT EXISTS gamedata_db.`inapppurchaseitem`
(
    Id VARCHAR(50) NOT NULL  comment '영수증id',
    Code int NOT NULL COMMENT '아이템 코드');
```


# Notice DB
## 레디스 공지 데이터베이스
```c#
        Notice notice= new Notice("공지1","공지1입니다","2023-01-01","false");
        await noticeRedis.RightPushAsync(notice,TimeSpan.FromDays(7));
        Notice notice2 = new Notice("공지2", "공지2입니다", "2023-01-01", "false");\
        await noticeRedis.RightPushAsync(notice2,TimeSpan.FromDays(7));
        Notice notice3= new Notice("공지3","공지3입니다","2023-01-01","false");
        await noticeRedis.RightPushAsync(notice3,TimeSpan.FromDays(7));
```