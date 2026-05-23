using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.API.Data
{
    public static class DataSeeder
    {
        private static readonly PasswordHasher<User> _hasher = new();
        private const string DefaultPassword = "Chnu2026!";

        public static async Task SeedAsync(AppDbContext db)
        {
            if (await db.Users.AnyAsync(u => u.Email == "kovalchuk.i@chnu.edu.ua"))
                return;

            // ── USERS ──────────────────────────────────────────────────────────
            var admin = MakeUser("admin.fi@chnu.edu.ua", "Адмін Факультету Інформатики",
                "Факультет інформатики", 0, UserRole.admin,
                "Відповідаю за технічну частину платформи CHNU-Connect.");

            var teacherKoval = MakeUser("kovalchuk.i@chnu.edu.ua", "Ковальчук Ірина Василівна",
                "Факультет інформатики", 0, UserRole.teacher,
                "Викладач кафедри комп'ютерних наук. Веду курси: Бази даних, Комп'ютерні мережі.");

            var teacherIvan = MakeUser("ivanchenko.o@chnu.edu.ua", "Іванченко Олексій Сергійович",
                "Факультет інформатики", 0, UserRole.teacher,
                "Доцент кафедри ПЗ. Захоплення: алгоритмічні змагання, open-source.");

            var petrenko = MakeUser("petrenko.m@chnu.edu.ua", "Петренко Максим Андрійович",
                "Факультет інформатики", 4, UserRole.student,
                "4-й курс, ФІ-41. Люблю бекенд і трохи DevOps.");

            var kovalenko = MakeUser("kovalenko.a@chnu.edu.ua", "Коваленко Анастасія Олегівна",
                "Факультет інформатики", 4, UserRole.student,
                "4-й курс, ФІ-41. Frontend + UI/UX.");

            var shevchenko = MakeUser("shevchenko.d@chnu.edu.ua", "Шевченко Дмитро Вікторович",
                "Факультет інформатики", 4, UserRole.student,
                "ФІ-41(2). Пишу на Rust у вільний час.");

            var melnyk = MakeUser("melnyk.o@chnu.edu.ua", "Мельник Олег Петрович",
                "Факультет інформатики", 4, UserRole.student,
                "ФІ-41(2). Геймдев та графіка — моє все.");

            var boyko = MakeUser("boyko.v@chnu.edu.ua", "Бойко Вікторія Іванівна",
                "Факультет інформатики", 4, UserRole.student,
                "ФІ-41(1). ML/Data Science ентузіаст.");

            var lysenko = MakeUser("lysenko.p@chnu.edu.ua", "Лисенко Павло Романович",
                "Факультет інформатики", 4, UserRole.student,
                "ФІ-41(2). Fullstack з React та .NET.");

            var moroz = MakeUser("moroz.k@chnu.edu.ua", "Мороз Катерина Миколаївна",
                "Факультет інформатики", 4, UserRole.student,
                "ФІ-41(1). Захоплююсь кібербезпекою.");

            var users = new[] { admin, teacherKoval, teacherIvan, petrenko, kovalenko, shevchenko, melnyk, boyko, lysenko, moroz };
            db.Users.AddRange(users);
            await db.SaveChangesAsync();

            // ── LESSON SLOTS ───────────────────────────────────────────────────
            var slot1 = new LessonSlot { PairNumber = 1, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(9, 20, 0) };
            var slot2 = new LessonSlot { PairNumber = 2, StartTime = new TimeSpan(9, 30, 0), EndTime = new TimeSpan(10, 50, 0) };
            var slot3 = new LessonSlot { PairNumber = 3, StartTime = new TimeSpan(11, 10, 0), EndTime = new TimeSpan(12, 30, 0) };
            var slot4 = new LessonSlot { PairNumber = 4, StartTime = new TimeSpan(12, 40, 0), EndTime = new TimeSpan(14, 0, 0) };
            var slot5 = new LessonSlot { PairNumber = 5, StartTime = new TimeSpan(14, 10, 0), EndTime = new TimeSpan(15, 30, 0) };
            var slot6 = new LessonSlot { PairNumber = 6, StartTime = new TimeSpan(15, 40, 0), EndTime = new TimeSpan(17, 0, 0) };
            db.LessonSlots.AddRange(slot1, slot2, slot3, slot4, slot5, slot6);
            await db.SaveChangesAsync();

            // ── GROUPS ─────────────────────────────────────────────────────────
            var groupFI41 = new Group
            {
                Name = "ФІ-41",
                Description = "Академічна група 4-го курсу факультету інформатики. Спеціальність: Комп'ютерні науки.",
                CreatorId = admin.Id,
                CuratorId = teacherKoval.Id,
                Type = GroupType.Academic,
                IsPrivate = false,
                CreatedAt = DateTime.UtcNow.AddMonths(-8),
            };

            var groupWeb = new Group
            {
                Name = "Веб-розробка 2026",
                Description = "Курс з сучасної веб-розробки: React, .NET, PostgreSQL, Docker. Практичні проекти та код-рев'ю щотижня.",
                CreatorId = teacherIvan.Id,
                CuratorId = teacherIvan.Id,
                Type = GroupType.Course,
                IsPrivate = false,
                CreatedAt = DateTime.UtcNow.AddMonths(-3),
            };

            var groupDekanat = new Group
            {
                Name = "Оголошення деканату ФІ",
                Description = "Офіційні оголошення деканату факультету інформатики. Сесія, розклад, накази.",
                CreatorId = admin.Id,
                Type = GroupType.Announcement,
                IsPrivate = false,
                CreatedAt = DateTime.UtcNow.AddMonths(-9),
            };

            db.Groups.AddRange(groupFI41, groupWeb, groupDekanat);
            await db.SaveChangesAsync();

            // ── SUBGROUPS ──────────────────────────────────────────────────────
            var sg1 = new SubGroup { Name = "ФІ-41(1)", GroupId = groupFI41.Id };
            var sg2 = new SubGroup { Name = "ФІ-41(2)", GroupId = groupFI41.Id };
            db.SubGroups.AddRange(sg1, sg2);
            await db.SaveChangesAsync();

            // Assign students to subgroups
            foreach (var u in new[] { petrenko, kovalenko, boyko, moroz })
                u.SubGroupId = sg1.Id;
            foreach (var u in new[] { shevchenko, melnyk, lysenko })
                u.SubGroupId = sg2.Id;
            await db.SaveChangesAsync();

            // ── SUBJECTS ───────────────────────────────────────────────────────
            var subjDB = new Subject { Name = "Бази даних", Description = "Реляційні БД, SQL, нормалізація, індексування, транзакції.", GroupId = groupFI41.Id, TeacherId = teacherKoval.Id, Semester = 7, CreatedAt = DateTime.UtcNow.AddMonths(-7) };
            var subjAlgo = new Subject { Name = "Алгоритми та структури даних", Description = "Складність алгоритмів, сортування, дерева, графи, динамічне програмування.", GroupId = groupFI41.Id, TeacherId = teacherIvan.Id, Semester = 7, CreatedAt = DateTime.UtcNow.AddMonths(-7) };
            var subjOOP = new Subject { Name = "Об'єктно-орієнтоване програмування", Description = "Патерни проектування, SOLID, рефакторинг, чистий код.", GroupId = groupFI41.Id, TeacherId = teacherIvan.Id, Semester = 7, CreatedAt = DateTime.UtcNow.AddMonths(-7) };
            var subjNet = new Subject { Name = "Комп'ютерні мережі", Description = "Стек TCP/IP, протоколи, маршрутизація, безпека мереж.", GroupId = groupFI41.Id, TeacherId = teacherKoval.Id, Semester = 7, CreatedAt = DateTime.UtcNow.AddMonths(-7) };
            var subjWeb = new Subject { Name = "Web-технології", Description = "HTML/CSS, JavaScript, REST API, фреймворки, деплой.", GroupId = groupFI41.Id, TeacherId = teacherIvan.Id, Semester = 7, CreatedAt = DateTime.UtcNow.AddMonths(-7) };
            db.Subjects.AddRange(subjDB, subjAlgo, subjOOP, subjNet, subjWeb);
            await db.SaveChangesAsync();

            // ── GROUP MEMBERS ──────────────────────────────────────────────────
            var studentMembers = new[] { petrenko, kovalenko, shevchenko, melnyk, boyko, lysenko, moroz };
            db.GroupMembers.AddRange(
                new GroupMember { GroupId = groupFI41.Id, UserId = admin.Id, Role = GroupMemberRole.Curator, JoinedAt = DateTime.UtcNow.AddMonths(-8) },
                new GroupMember { GroupId = groupFI41.Id, UserId = teacherKoval.Id, Role = GroupMemberRole.Curator, JoinedAt = DateTime.UtcNow.AddMonths(-8) },
                new GroupMember { GroupId = groupFI41.Id, UserId = teacherIvan.Id, Role = GroupMemberRole.Assistant, JoinedAt = DateTime.UtcNow.AddMonths(-7) }
            );
            foreach (var s in studentMembers)
                db.GroupMembers.Add(new GroupMember { GroupId = groupFI41.Id, UserId = s.Id, Role = GroupMemberRole.Student, JoinedAt = DateTime.UtcNow.AddMonths(-8) });

            db.GroupMembers.AddRange(
                new GroupMember { GroupId = groupWeb.Id, UserId = teacherIvan.Id, Role = GroupMemberRole.Curator, JoinedAt = DateTime.UtcNow.AddMonths(-3) },
                new GroupMember { GroupId = groupWeb.Id, UserId = petrenko.Id, Role = GroupMemberRole.Student, JoinedAt = DateTime.UtcNow.AddMonths(-3) },
                new GroupMember { GroupId = groupWeb.Id, UserId = kovalenko.Id, Role = GroupMemberRole.Student, JoinedAt = DateTime.UtcNow.AddMonths(-3) },
                new GroupMember { GroupId = groupWeb.Id, UserId = lysenko.Id, Role = GroupMemberRole.Student, JoinedAt = DateTime.UtcNow.AddMonths(-3) },
                new GroupMember { GroupId = groupWeb.Id, UserId = boyko.Id, Role = GroupMemberRole.Student, JoinedAt = DateTime.UtcNow.AddMonths(-2) },
                new GroupMember { GroupId = groupDekanat.Id, UserId = admin.Id, Role = GroupMemberRole.Curator, JoinedAt = DateTime.UtcNow.AddMonths(-9) },
                new GroupMember { GroupId = groupDekanat.Id, UserId = teacherKoval.Id, Role = GroupMemberRole.Assistant, JoinedAt = DateTime.UtcNow.AddMonths(-9) }
            );
            foreach (var s in studentMembers)
                db.GroupMembers.Add(new GroupMember { GroupId = groupDekanat.Id, UserId = s.Id, Role = GroupMemberRole.Student, JoinedAt = DateTime.UtcNow.AddMonths(-8) });

            await db.SaveChangesAsync();

            // ── SCHEDULE ───────────────────────────────────────────────────────
            // Week 1 (Чисельник)
            db.Schedules.AddRange(
                // Понеділок
                Sch(groupFI41.Id, subjDB.Id, null, LessonType.Lecture, WeekType.First, DayOfWeek.Monday, slot2.Id, "Бази даних", "Ковальчук І.В.", "А-301"),
                Sch(groupFI41.Id, subjWeb.Id, sg1.Id, LessonType.Practice, WeekType.First, DayOfWeek.Monday, slot3.Id, "Web-технології", "Іванченко О.С.", "Лаб А-112"),

                // Вівторок
                Sch(groupFI41.Id, subjAlgo.Id, null, LessonType.Lecture, WeekType.First, DayOfWeek.Tuesday, slot1.Id, "Алгоритми та структури даних", "Іванченко О.С.", "А-215"),
                Sch(groupFI41.Id, subjOOP.Id, sg1.Id, LessonType.Practice, WeekType.First, DayOfWeek.Tuesday, slot2.Id, "ООП та патерни проектування", "Іванченко О.С.", "Лаб А-115"),

                // Середа
                Sch(groupFI41.Id, subjNet.Id, null, LessonType.Lecture, WeekType.First, DayOfWeek.Wednesday, slot3.Id, "Комп'ютерні мережі", "Ковальчук І.В.", "А-301"),
                Sch(groupFI41.Id, subjDB.Id, sg2.Id, LessonType.Practice, WeekType.First, DayOfWeek.Wednesday, slot4.Id, "Бази даних", "Ковальчук І.В.", "Лаб А-112"),

                // Четвер
                Sch(groupFI41.Id, subjWeb.Id, sg2.Id, LessonType.Practice, WeekType.First, DayOfWeek.Thursday, slot2.Id, "Web-технології", "Іванченко О.С.", "Лаб А-112"),
                Sch(groupFI41.Id, subjAlgo.Id, sg1.Id, LessonType.Practice, WeekType.First, DayOfWeek.Thursday, slot3.Id, "Алгоритми та структури даних", "Іванченко О.С.", "Лаб Б-204"),

                // П'ятниця
                Sch(groupFI41.Id, subjOOP.Id, sg2.Id, LessonType.Practice, WeekType.First, DayOfWeek.Friday, slot4.Id, "ООП та патерни проектування", "Іванченко О.С.", "Лаб А-115"),
                Sch(groupFI41.Id, subjNet.Id, sg1.Id, LessonType.Practice, WeekType.First, DayOfWeek.Friday, slot5.Id, "Комп'ютерні мережі", "Ковальчук І.В.", "Лаб Б-201"),

                // Week 2 (Знаменник)
                Sch(groupFI41.Id, subjDB.Id, null, LessonType.Lecture, WeekType.Second, DayOfWeek.Monday, slot2.Id, "Бази даних", "Ковальчук І.В.", "А-301"),
                Sch(groupFI41.Id, subjAlgo.Id, sg2.Id, LessonType.Practice, WeekType.Second, DayOfWeek.Monday, slot3.Id, "Алгоритми та структури даних", "Іванченко О.С.", "Лаб Б-204"),

                Sch(groupFI41.Id, subjAlgo.Id, null, LessonType.Lecture, WeekType.Second, DayOfWeek.Tuesday, slot1.Id, "Алгоритми та структури даних", "Іванченко О.С.", "А-215"),
                Sch(groupFI41.Id, subjOOP.Id, sg2.Id, LessonType.Practice, WeekType.Second, DayOfWeek.Tuesday, slot2.Id, "ООП та патерни проектування", "Іванченко О.С.", "Лаб А-115"),

                Sch(groupFI41.Id, subjNet.Id, null, LessonType.Lecture, WeekType.Second, DayOfWeek.Wednesday, slot3.Id, "Комп'ютерні мережі", "Ковальчук І.В.", "А-301"),
                Sch(groupFI41.Id, subjWeb.Id, sg1.Id, LessonType.Practice, WeekType.Second, DayOfWeek.Wednesday, slot4.Id, "Web-технології", "Іванченко О.С.", "Лаб А-112"),

                Sch(groupFI41.Id, subjDB.Id, sg1.Id, LessonType.Practice, WeekType.Second, DayOfWeek.Thursday, slot2.Id, "Бази даних", "Ковальчук І.В.", "Лаб А-112"),
                Sch(groupFI41.Id, subjNet.Id, sg2.Id, LessonType.Practice, WeekType.Second, DayOfWeek.Thursday, slot3.Id, "Комп'ютерні мережі", "Ковальчук І.В.", "Лаб Б-201"),

                Sch(groupFI41.Id, subjOOP.Id, sg1.Id, LessonType.Practice, WeekType.Second, DayOfWeek.Friday, slot4.Id, "ООП та патерни проектування", "Іванченко О.С.", "Лаб А-115"),
                Sch(groupFI41.Id, subjWeb.Id, sg2.Id, LessonType.Practice, WeekType.Second, DayOfWeek.Friday, slot5.Id, "Web-технології", "Іванченко О.С.", "Лаб А-112")
            );
            await db.SaveChangesAsync();

            // ── EVENTS ─────────────────────────────────────────────────────────
            var events = new[]
            {
                new Event
                {
                    Title = "Хакатон «ChernivtsiHack 2026»",
                    Description = "Щорічний університетський хакатон! 24 години, командна робота, 5 треків: веб, мобайл, AI, кібербезпека, геймдев. Призи для топ-3 команд: ноутбуки, сертифікати, стажування.\n\nРеєстрація команд до 1 червня. Формат: 2–5 осіб у команді. Місце проведення: Головний корпус ЧНУ, актова зала.",
                    StartTime = DateTime.UtcNow.AddDays(12).Date.AddHours(9),
                    EndTime = DateTime.UtcNow.AddDays(13).Date.AddHours(9),
                    CreatorId = admin.Id,
                    IsPublic = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                },
                new Event
                {
                    Title = "Науково-практична конференція студентів ФІ",
                    Description = "Щорічна студентська конференція кафедри комп'ютерних наук. Секції: прикладне ПЗ, машинне навчання, кібербезпека, web-технології.\n\nДедлайн подачі тез: 15 червня. Формат доповіді: 10 хв + 5 хв запитань. Кращі роботи рекомендуються до публікації у збірнику.",
                    StartTime = DateTime.UtcNow.AddDays(25).Date.AddHours(10),
                    EndTime = DateTime.UtcNow.AddDays(25).Date.AddHours(17),
                    CreatorId = teacherKoval.Id,
                    IsPublic = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                },
                new Event
                {
                    Title = "Воркшоп «Git & GitHub для командної роботи»",
                    Description = "Практичний воркшоп для всіх, хто хоче впевнено використовувати Git у командних проектах.\n\nПрограма: branching strategies, pull requests, code review, CI/CD основи. Потрібен ноутбук з встановленим Git. Рівень: початківець та середній.",
                    StartTime = DateTime.UtcNow.AddDays(5).Date.AddHours(14),
                    EndTime = DateTime.UtcNow.AddDays(5).Date.AddHours(17),
                    CreatorId = teacherIvan.Id,
                    IsPublic = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                },
                new Event
                {
                    Title = "День відкритих дверей ЧНУ — Факультет інформатики",
                    Description = "Запрошуємо майбутніх абітурієнтів на День відкритих дверей факультету інформатики! Вас чекають: екскурсія лабораторіями, зустріч зі студентами та викладачами, демо студентських проектів.\n\nВхід вільний. Захід проходить у главному корпусі, аудиторія А-101.",
                    StartTime = DateTime.UtcNow.AddDays(18).Date.AddHours(11),
                    EndTime = DateTime.UtcNow.AddDays(18).Date.AddHours(15),
                    CreatorId = admin.Id,
                    IsPublic = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                },
                new Event
                {
                    Title = "Консультація перед заліком з Баз даних",
                    Description = "Ірина Василівна проводить онлайн-консультацію перед заліком. Підготуйте питання заздалегідь. Посилання на Zoom буде надіслано у групу ФІ-41 за 30 хвилин до початку.",
                    StartTime = DateTime.UtcNow.AddDays(3).Date.AddHours(16),
                    EndTime = DateTime.UtcNow.AddDays(3).Date.AddHours(18),
                    CreatorId = teacherKoval.Id,
                    IsPublic = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                },
            };
            db.Events.AddRange(events);
            await db.SaveChangesAsync();

            // ── POSTS ──────────────────────────────────────────────────────────
            var post1 = Post(teacherIvan.Id, "Нагадую всім студентам групи ФІ-41: лабораторна робота №4 з ООП має бути здана до кінця тижня. Тема: реалізація патернів «Спостерігач» та «Стратегія». Звіт у форматі PDF, код — у GitHub репозиторій з посиланням у звіті. Питання — пишіть у чат групи.", DateTime.UtcNow.AddDays(-8));
            var post2 = Post(petrenko.Id, "Хтось може пояснити різницю між LEFT JOIN та LEFT OUTER JOIN в PostgreSQL? Читав документацію, але все одно плутаюсь. На парах Іриною Василівною щось пропустив 🙈", DateTime.UtcNow.AddDays(-7));
            var post3 = Post(teacherKoval.Id, "Для підготовки до заліку з БД рекомендую прочитати розділи 12-15 з підручника Рамакрішнана. Особлива увага — індекси B-дерева та хеш-індекси, транзакції та рівні ізоляції. Це буде на заліку 100%.", DateTime.UtcNow.AddDays(-6));
            var post4 = Post(boyko.Id, "Знайшла чудовий безкоштовний курс з Machine Learning від Stanford на Coursera — всього 11 тижнів, дуже структурований. Якщо хтось хоче разом проходити та обговорювати — пишіть! Можемо створити навчальну групу 🤝", DateTime.UtcNow.AddDays(-5));
            var post5 = Post(shevchenko.Id, "Зробив невеличкий CLI-інструмент на Rust для автоматичного форматування SQL-запитів. Якщо комусь цікаво — можу поділитися посиланням на GitHub. Поки що підтримує PostgreSQL та MySQL синтаксис.", DateTime.UtcNow.AddDays(-5));
            var post6 = Post(moroz.Id, "Порада всім: встановіть розширення «SQLTools» для VS Code — дуже зручно підключатися до PostgreSQL прямо з редактора, бачити схеми таблиць та виконувати запити. Значно зручніше ніж pgAdmin для повсякденної роботи.", DateTime.UtcNow.AddDays(-4));
            var post7 = Post(kovalenko.Id, "Зробила дизайн макет для нашого дипломного проекту у Figma. Хто в команді — зайдіть подивіться та залиште коментарі. Буду вдячна за фідбек! Дедлайн узгодження до п'ятниці.", DateTime.UtcNow.AddDays(-4));
            var post8 = Post(lysenko.Id, "Питання до тих хто вже здав ООП лабу: як ви реалізували патерн «Спостерігач»? Я зробив через інтерфейс IObserver, але Олексій Сергійович сказав переробити... Не розумію що не так.", DateTime.UtcNow.AddDays(-3));
            var post9 = Post(melnyk.Id, "Сьогодні на парі з мереж розібрали як працює TLS handshake. Реально цікаво! Якщо хочете краще зрозуміти — рекомендую погратися з Wireshark, дуже наочно видно всі пакети під час з'єднання.", DateTime.UtcNow.AddDays(-3));
            var post10 = Post(teacherIvan.Id, "🎉 Вітаю команду «ByteForce» (Петренко, Шевченко, Лисенко) з 1-м місцем на регіональному конкурсі з програмування! Пишаємось нашими студентами. Чекаємо на всеукраїнський тур!", DateTime.UtcNow.AddDays(-2));
            var post11 = Post(petrenko.Id, "Налаштував Docker Compose для нашого дипломного проекту — тепер підняти весь стек (React + .NET API + PostgreSQL + Redis) одною командою. Якщо хтось хоче допомогти з CI/CD pipeline — пишіть, є ідеї!", DateTime.UtcNow.AddDays(-2));
            var post12 = Post(admin.Id, "📢 Увага всім студентам! Розклад на наступний тиждень змінено через конференцію. Вівторок 3 червня — усі пари перенесено. Оновлений розклад вже у системі. Перевіряйте у розділі «Розклад».", DateTime.UtcNow.AddDays(-1));
            var post13 = Post(kovalenko.Id, "Нарешті розібралась з Flexbox і CSS Grid! Раніше постійно гуглила, а тепер вже верстаю інтуїтивно. Ресурс який допоміг найбільше — Flexbox Froggy та Grid Garden (ігрові тренажери). Раджу всім хто вчить CSS!", DateTime.UtcNow.AddHours(-18));
            var post14 = Post(boyko.Id, "Хтось пробував FastAPI замість Flask/Django для Python бекенду? Цікавить ваш досвід — чи варто переходити? Для мого ML-проекту потрібен простий REST API.", DateTime.UtcNow.AddHours(-10));
            var post15 = Post(moroz.Id, "Пройшла онлайн CTF змагання вчора — дуже захопливо! Якщо хтось цікавиться кібербезпекою, рекомендую spook.ie та picoCTF для початку. Готова поділитись write-up'ами для деяких задач.", DateTime.UtcNow.AddHours(-3));

            db.Posts.AddRange(post1, post2, post3, post4, post5, post6, post7, post8, post9, post10, post11, post12, post13, post14, post15);
            await db.SaveChangesAsync();

            // ── COMMENTS ───────────────────────────────────────────────────────
            db.Comments.AddRange(
                // на post1 (лаба ООП)
                Cmt(kovalenko.Id, post1.Id, "Дякую за нагадування! А можна уточнити — звіт обов'язково PDF, чи Word теж підійде?", DateTime.UtcNow.AddDays(-8).AddHours(1)),
                Cmt(teacherIvan.Id, post1.Id, "Тільки PDF, щоб форматування не їхало. Word не приймаю.", DateTime.UtcNow.AddDays(-8).AddHours(2)),
                Cmt(lysenko.Id, post1.Id, "А кількість сторінок є обмеження? Мій звіт вже 20 сторінок...", DateTime.UtcNow.AddDays(-7).AddHours(10)),
                Cmt(teacherIvan.Id, post1.Id, "Мінімум 10, максимум не обмежений. Головне — якість, не кількість.", DateTime.UtcNow.AddDays(-7).AddHours(11)),

                // на post2 (JOIN питання)
                Cmt(moroz.Id, post2.Id, "В PostgreSQL це синоніми! LEFT JOIN і LEFT OUTER JOIN роблять абсолютно одне й те саме. OUTER — просто для читабельності.", DateTime.UtcNow.AddDays(-7).AddHours(1)),
                Cmt(teacherKoval.Id, post2.Id, "Максим, правильно! У SQL стандарті OUTER — необов'язкове ключове слово. Всі INNER/LEFT/RIGHT JOIN мають свій OUTER варіант, але це одне й те саме.", DateTime.UtcNow.AddDays(-7).AddHours(3)),
                Cmt(petrenko.Id, post2.Id, "Дякую! Тепер зрозуміло чому в різних джерелах по-різному написано 😅", DateTime.UtcNow.AddDays(-7).AddHours(4)),

                // на post4 (ML курс)
                Cmt(shevchenko.Id, post4.Id, "Звучить цікаво! А який саме курс? Andrew Ng чи новий?", DateTime.UtcNow.AddDays(-5).AddHours(2)),
                Cmt(boyko.Id, post4.Id, "Новий ML спеціалізація від Ng — 2023 версія. Значно оновлений порівняно зі старим. Включає scikit-learn, TensorFlow.", DateTime.UtcNow.AddDays(-5).AddHours(3)),
                Cmt(petrenko.Id, post4.Id, "Я теж хочу! Давай у суботу стартуємо — можемо зідзвонитися і обговорити перший тиждень.", DateTime.UtcNow.AddDays(-5).AddHours(5)),

                // на post5 (Rust CLI)
                Cmt(lysenko.Id, post5.Id, "О, поділись! Rust цікавить давно, але поки що тільки Hello World зробив 😄", DateTime.UtcNow.AddDays(-5).AddHours(1)),
                Cmt(teacherIvan.Id, post5.Id, "Дмитре, класна ідея! Якщо опублікуєш з ReadMe та тестами — можна зарахувати як індивідуальне завдання до ООП.", DateTime.UtcNow.AddDays(-4).AddHours(9)),
                Cmt(shevchenko.Id, post5.Id, "@Іванченко Олексій Сергійович дякую! Завтра оформлю і скину посилання.", DateTime.UtcNow.AddDays(-4).AddHours(10)),

                // на post8 (ООП патерн)
                Cmt(petrenko.Id, post8.Id, "Я робив через делегати і events в C# — це набагато чистіше ніж ручний IObserver. Можу показати код.", DateTime.UtcNow.AddDays(-3).AddHours(1)),
                Cmt(teacherIvan.Id, post8.Id, "Павле, для цього завдання хочу саме класичний GoF патерн з інтерфейсами, щоб зрозуміти принцип. Делегати — це мовна абстракція поверх нього.", DateTime.UtcNow.AddDays(-3).AddHours(2)),
                Cmt(lysenko.Id, post8.Id, "Зрозумів, дякую! Тобто треба Subject/Observer інтерфейси руками, не через вбудовані механізми мови?", DateTime.UtcNow.AddDays(-3).AddHours(3)),
                Cmt(teacherIvan.Id, post8.Id, "Саме так. Покажіть що ви розумієте патерн — потім можна і через делегати показати як «оптимізація».", DateTime.UtcNow.AddDays(-3).AddHours(4)),

                // на post10 (вітання)
                Cmt(petrenko.Id, post10.Id, "Дякуємо! Було дуже інтенсивно, але кайфово 🔥 Особлива подяка Дмитру за алгоритм на задачі з графами!", DateTime.UtcNow.AddDays(-2).AddHours(1)),
                Cmt(shevchenko.Id, post10.Id, "Команда вогонь! Лисенку окремий респект за дебагінг о 3-й ночі 😂", DateTime.UtcNow.AddDays(-2).AddHours(2)),
                Cmt(boyko.Id, post10.Id, "Молодці хлопці! Наступного разу беріть мене в команду 😄", DateTime.UtcNow.AddDays(-2).AddHours(3)),
                Cmt(moroz.Id, post10.Id, "Неймовірно! Пишаюсь нашою групою ❤️", DateTime.UtcNow.AddDays(-2).AddHours(4)),

                // на post11 (Docker)
                Cmt(kovalenko.Id, post11.Id, "Класно! А Redis навіщо? Кешування чи сесії?", DateTime.UtcNow.AddDays(-2).AddHours(2)),
                Cmt(petrenko.Id, post11.Id, "Поки що для кешування відповідей API — деякі запити до БД важкі. Потім може і для сесій використаємо.", DateTime.UtcNow.AddDays(-2).AddHours(3)),

                // на post14 (FastAPI)
                Cmt(shevchenko.Id, post14.Id, "FastAPI дуже швидкий і зручний! Автоматична документація Swagger, async підтримка. Для ML API — ідеальний вибір.", DateTime.UtcNow.AddHours(-9)),
                Cmt(teacherIvan.Id, post14.Id, "Погоджуюсь з Дмитром. FastAPI зараз де-факто стандарт для Python ML сервісів. Django надто великий для простого API.", DateTime.UtcNow.AddHours(-8)),
                Cmt(boyko.Id, post14.Id, "Дякую! Тоді FastAPI і починаю. Є вже базовий досвід з Flask, тому буде не складно перейти.", DateTime.UtcNow.AddHours(-7)),

                // на post15 (CTF)
                Cmt(melnyk.Id, post15.Id, "О! Я теж брав участь у picoCTF минулого року. Дуже класна платформа для початківців. Які задачі вирішила?", DateTime.UtcNow.AddHours(-2)),
                Cmt(moroz.Id, post15.Id, "Web exploitation та cryptography переважно. Forensics теж спробувала — цікаво, але складніше. Якщо хочеш — можемо разом наступний CTF пройти!", DateTime.UtcNow.AddHours(-1))
            );
            await db.SaveChangesAsync();
        }

        // ── Helpers ────────────────────────────────────────────────────────────
        private static User MakeUser(string email, string fullName, string faculty, int course, UserRole role, string bio)
        {
            var u = new User { Email = email, FullName = fullName, Faculty = faculty, Course = course == 0 ? null : course, Role = role, Bio = bio, IsEmailConfirmed = true, CreatedAt = DateTime.UtcNow.AddMonths(-9) };
            u.PasswordHash = new PasswordHasher<User>().HashPassword(u, DefaultPassword);
            return u;
        }

        private static Schedule Sch(int groupId, int? subjectId, int? subGroupId, LessonType type, WeekType week, DayOfWeek day, int slotId, string subjectName, string teacherName, string location)
            => new Schedule { GroupId = groupId, SubjectId = subjectId, SubGroupId = subGroupId, Type = type, Week = week, Day = day, SlotId = slotId, SubjectName = subjectName, TeacherName = teacherName, Location = location };

        private static Post Post(int userId, string content, DateTime createdAt)
            => new Post { UserId = userId, Content = content, CreatedAt = createdAt };

        private static Comment Cmt(int userId, int postId, string content, DateTime createdAt)
            => new Comment { UserId = userId, PostId = postId, Content = content, CreatedAt = createdAt };
    }
}
