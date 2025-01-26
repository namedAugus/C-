1. Base on .net 8.0

2. 数据库设计如下(以mysql为例)

```sql
CREATE DATABASE game;
DEFAULT CHARACTER SET utf8mb4
DEFAULT COLLATE utf8mb4_unicode_ci;
use game;

-- 用户表
create table account(
	id int auto_increment primary key , 
	username varchar(20) comment '账户', 
	`password` varchar(20) comment '密码'
	
);
	
-- 角色表
create table player(
	id int auto_increment primary key ,
	username varchar(20) comment '玩家account'
	hp float comment '玩家血量',
	mp float comment '玩家蓝量',
	att float comment '玩家攻击力',
	def float comment '玩家防御力',
	x float comment '玩家x轴位置',
	y float comment '玩家y轴位置',
    z float comment '玩家z轴位置',
	job varchar(10) comment '玩家职业',
	`data` varchar(255) comment '玩家数据(如装备、金币、皮肤)'
);
```

3. NetFramework为客户端（推荐unity2020 lts 长期支持版）
4. GameFramework为服务端，部署好数据库后运行即可
5. 客户端运行TankScene 即可（其他都是测试Scene）
