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
) COMMENT '계정 정보 테이블';
```   
   
<br>  
<br>  
   
   
# Game DB
  
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
    Count int NOT NULL COMMENT '아이템 갯수',
);
```

   ## inapppurchasereceipt 테이블
인앱결제 영수증 테이블
```sql
  CREATE TABLE IF NOT EXISTS gamedata_db.`inapppurchasereceipt`
(
    Id bigint not null auto_increment primary key comment '영수증id',
    Email VARCHAR(50) NOT NULL COMMENT '이메일',
    Title VARCHAR(100) NOT NULL COMMENT  '제목',
    Content VARCHAR(100) NOT NULL COMMENT '내용',
    Time datetime default current_timestamp comment '발송날짜',
    ExpiryTime int comment '유효기간'
);
```
   ## inapppurchaseitem 테이블
인앱결제 아이템 테이블
```sql
  CREATE TABLE IF NOT EXISTS gamedata_db.`inapppurchaseitem`
(
    Id bigint not null auto_increment primary key comment '영수증id',
    Code int NOT NULL COMMENT '아이템 코드',
    Count int NOT NULL COMMENT '아이템 갯수',
);
```